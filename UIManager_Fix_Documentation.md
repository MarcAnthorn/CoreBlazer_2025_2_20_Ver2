# UIManager 场景切换错误修复说明

## 问题描述
在场景切换时出现以下错误：
```
Cannot set the parent of the GameObject 'MainPanel(Clone)' while its new parent 'PanelFather' is being destroyed
```

这个错误发生在 `GameMainPanel.OnDestroy()` 方法中调用 `UIManager.Instance.ShowPanel<MainPanel>()` 时，此时 `PanelFather` 对象正在被销毁。

## 根本原因
1. 在场景切换时，Unity会销毁当前场景中的所有GameObjects
2. `PanelFather` 对象被标记为销毁状态
3. 同时 `GameMainPanel.OnDestroy()` 被调用，尝试创建新的 `MainPanel`
4. UIManager 尝试将新创建的面板设置为正在被销毁的 `PanelFather` 的子对象
5. Unity 不允许将对象设置为正在被销毁对象的子对象，因此抛出错误

## 修复方案

### 1. UIManager.cs 修改
- **添加安全检查**：在所有 `ShowPanel` 方法中添加父对象有效性检查
- **父对象状态验证**：检查 `fatherTransform` 是否为空或正在被销毁
- **动态父对象重建**：如果 `PanelFather` 丢失，尝试重新查找或创建
- **实例化后再检查**：在面板实例化后再次验证父对象状态
- **添加清理方法**：提供 `CleanupAllPanels()` 方法用于场景切换时清理

### 2. GameMainPanel.cs 修改
- **安全的面板显示**：使用延迟协程避免在 OnDestroy 期间直接显示面板
- **场景状态检查**：检查场景是否正在卸载
- **应用程序状态验证**：确保应用程序仍在运行状态

### 3. 新增 SceneTransitionUIManager.cs
- **场景事件监听**：监听场景加载和卸载事件
- **自动UI清理**：在场景卸载时自动清理所有UI面板
- **生命周期管理**：跨场景保持活跃状态

## 修改详情

### UIManager.cs 的主要改进：

```csharp
// 安全检查示例
if (fatherTransform == null || fatherTransform.gameObject == null)
{
    Debug.LogWarning("PanelFather is null or destroyed, attempting to find or recreate it");
    fatherTransform = GameObject.Find("PanelFather")?.transform;
    if (fatherTransform == null)
    {
        Debug.LogError("Cannot find PanelFather, unable to show panel");
        return null;
    }
}

// 活跃状态检查
if (fatherTransform.gameObject.activeInHierarchy == false)
{
    Debug.LogWarning($"PanelFather is being destroyed or inactive, cannot show panel {typeof(T).Name}");
    return null;
}
```

### GameMainPanel.cs 的安全改进：

```csharp
// 延迟显示面板
private System.Collections.IEnumerator DelayedShowMainPanel()
{
    yield return new WaitForEndOfFrame();
    
    if (Application.isPlaying && UIManager.Instance != null)
    {
        var mainPanel = UIManager.Instance.ShowPanel<MainPanel>();
        if (mainPanel == null)
        {
            Debug.LogWarning("Failed to show MainPanel after GameMainPanel destruction");
        }
    }
}
```

## 使用方法

### 自动修复
- 修改后的代码会自动处理场景切换时的UI错误
- 不需要手动干预，系统会自动进行安全检查和清理

### 手动清理（如果需要）
```csharp
// 在场景切换前手动清理UI
if (UIManager.Instance != null)
{
    UIManager.Instance.CleanupAllPanels();
}
```

### 场景切换时的最佳实践
1. 确保 `SceneTransitionUIManager` 存在于第一个场景中
2. 避免在 `OnDestroy` 方法中直接调用 `ShowPanel`
3. 使用延迟调用或协程来处理UI显示逻辑

## 预防措施

### 1. 代码层面
- 始终检查父对象的有效性
- 使用延迟调用处理销毁期间的UI操作
- 监听场景事件进行主动清理

### 2. 设计层面
- 避免在对象销毁期间创建新的UI
- 使用事件系统解耦UI显示逻辑
- 确保UI管理器的生命周期管理正确

### 3. 调试层面
- 添加详细的日志输出
- 使用条件编译指令控制调试信息
- 监控UI对象的创建和销毁

## 测试验证

### 测试场景
1. 正常场景切换
2. 快速连续场景切换
3. 应用程序退出时的清理
4. 异常情况下的错误处理

### 验证要点
- 不再出现 "Cannot set the parent" 错误
- UI面板正确显示和隐藏
- 内存泄漏检查
- 性能影响评估

## 注意事项

1. **性能考虑**：增加的安全检查可能有轻微的性能开销，但在可接受范围内
2. **向后兼容性**：修改保持了原有的API接口，不影响现有代码
3. **调试信息**：添加了详细的日志输出，便于调试和监控
4. **错误恢复**：系统能够从错误状态中恢复，提高了稳定性

## 相关文件

- `Assets/Scripts/Managers/UIManager.cs` - 主要的UI管理器修改
- `Assets/Scripts/UIScripts/PanelScripts/GameMainPanel.cs` - 事件面板的安全修改
- `Assets/Scripts/Managers/SceneTransitionUIManager.cs` - 新增的场景切换管理器
- `UIManager_Fix_Documentation.md` - 本文档

## 总结

通过这些修改，解决了UIManager在场景切换时的错误问题，提高了系统的稳定性和容错能力。系统现在能够：

1. 安全地处理场景切换期间的UI操作
2. 自动清理和恢复UI状态
3. 提供详细的错误信息和调试支持
4. 保持原有功能的完整性

这些改进确保了游戏在各种场景切换情况下都能稳定运行，而不会出现UI相关的错误。
