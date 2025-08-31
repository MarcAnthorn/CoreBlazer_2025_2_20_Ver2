# 复活数据管理系统使用说明

## 概述
根据SaveManager架构和EventIterator模式，为CoreBlazer游戏创建了一个复活时自动储存游戏并显示所有储存的事件与互动过的物体的系统。

## 系统组成

### 1. ReviveDataManager.cs
**位置**: `Assets/Scripts/Managers/ReviveDataManager.cs`
**功能**: 独立的复活数据管理器，类似EventIterator的使用模式
**特性**:
- 单例模式，自动初始化
- 按R键测试功能
- 完整的复活数据追踪和历史记录
- JSON格式数据持久化存储
- 与EventIterator集成，获取已完成事件信息

### 2. SaveManager扩展功能
**位置**: `Assets/Scripts/Managers/SaveManager.cs`
**新增方法**: `SaveGameOnReviveAndShowData()`
**功能**: 在SaveManager中直接集成复活数据显示功能
**特性**:
- 保存游戏后立即显示详细数据分析
- 兼容现有存档系统
- 与EventIterator集成显示事件完成情况
- 全面的游戏进度统计

### 3. PlayerController集成
**位置**: `Assets/Scripts/Player/PlayerController.cs`
**修改方法**: `SwitchSceneCallback()`
**功能**: 在玩家复活时自动调用数据管理系统
**特性**:
- 优先使用ReviveDataManager，回退到SaveManager方法
- 使用反射避免编译时依赖问题
- 错误处理和日志记录

### 4. 测试脚本
**ReviveDataTester.cs**: ReviveDataManager的专用测试脚本
**SaveManagerReviveTester.cs**: SaveManager复活功能的测试脚本

## 功能详情

### 复活时显示的数据包括：

#### 1. 基本复活信息
- 复活时间戳
- 复活时的SAN值和HP值
- 当前关卡信息
- 玩家位置

#### 2. 事件完成情况
- 总事件数和已完成事件数
- 事件完成率
- 起始事件和选项事件分别统计
- 已完成事件ID列表
- 最近完成的事件详情

#### 3. 交互物体状态
- 休息点激活情况
- 灯塔激活情况
- 关键点触发情况
- 道具点收集情况
- 门锁解锁情况
- 总体交互完成率

#### 4. 道具装备信息
- 道具种类数和总数量
- 持有道具详细列表
- 装备数量和总耐久
- 持有装备详细列表

#### 5. AVG剧情进度
- NPC事件AVG触发统计
- 安全屋AVG触发统计
- 已触发的AVG列表

#### 6. 总体游戏统计
- 事件完成率
- 交互完成率
- AVG完成率
- 总体游戏完成度
- 存档文件信息

## 使用方法

### 自动触发
当玩家死亡复活时，系统会自动调用复活数据管理功能，无需手动操作。

### 手动测试
1. **ReviveDataManager测试**:
   - F9: 模拟复活
   - F10: 显示复活数据
   - F11: 清空复活历史
   - R: EventIterator测试

2. **SaveManager测试**:
   - F7: 普通保存游戏
   - F8: 复活保存并显示数据

### 编程接口

#### ReviveDataManager
```csharp
// 手动触发复活数据处理
ReviveDataManager.Instance.OnPlayerRevive();

// 获取复活历史统计
var stats = ReviveDataManager.Instance.GetReviveHistoryStats();

// 获取格式化的复活数据文本
string text = ReviveDataManager.Instance.GetFormattedReviveDataText();

// 清空复活历史
ReviveDataManager.Instance.ClearReviveHistory();
```

#### SaveManager
```csharp
// 复活时保存并显示数据
SaveManager.Instance.SaveGameOnReviveAndShowData();

// 普通保存（不显示详细数据）
SaveManager.Instance.SaveGame();
```

## 数据存储

### 复活历史数据
**文件路径**: `Application.persistentDataPath + "/revive_data_save.json"`
**内容**: 复活历史记录、统计信息、数据快照

### 游戏存档数据
保持原有SaveManager的存档文件结构不变：
- 玩家属性存档
- 玩家位置存档
- 关卡进度存档
- 装备存档
- 道具存档
- AVG分发存档
- RestPoint与LightHouse存档

## 集成说明

### 与EventIterator的集成
系统完全兼容并依赖EventIterator.cs的功能：
- 获取已完成事件信息
- 使用事件统计功能
- 保持相同的数据显示风格

### 与现有系统的兼容性
- 不影响现有的SaveManager保存逻辑
- 不修改现有的数据结构
- 通过扩展方法添加新功能
- 保持向后兼容

## 注意事项

1. **编译依赖**: ReviveDataManager可能需要Unity重新编译才能被其他脚本识别
2. **性能考虑**: 复活时的数据显示可能产生较多日志输出，可通过调试开关控制
3. **存储空间**: 复活历史记录会占用一定存储空间，系统自动限制在50条记录内
4. **测试环境**: 建议在开发环境中启用详细日志，发布时可关闭调试输出

## 故障排除

### ReviveDataManager未找到
- 确保脚本已被Unity编译
- 检查命名空间和类名是否正确
- 系统会自动回退到SaveManager方法

### 数据显示不完整
- 检查LoadManager.Instance是否正确初始化
- 确保EventIterator.Instance可用
- 验证GameLevelManager.Instance状态

### 保存失败
- 检查文件权限
- 确保存储路径可写
- 查看Unity Console中的错误信息

## 扩展建议

1. **UI界面**: 可以创建专门的UI面板显示复活数据
2. **数据分析**: 可以添加更多的数据分析功能，如完成时间统计
3. **导出功能**: 可以添加导出复活数据到外部文件的功能
4. **图表显示**: 可以使用图表库显示数据趋势
