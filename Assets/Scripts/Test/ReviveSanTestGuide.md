# 复活SAN奖励系统测试指南

## 概述
本测试系统提供了完整的复活SAN奖励功能测试环境，包括交互对象模拟、SAN奖励计算、数据持久化等功能。

**重要更新**: SAN奖励系统现已**改为复活后触发**，在玩家死亡复活时自动计算和分发奖励。

## 复活触发机制

### 自动触发流程
1. **玩家死亡** → 不显示死亡面板，直接进入复活逻辑 (PlayerController.OnPlayerDead)
2. **场景切换** → 自动切换到安全屋场景 (PlayerController.SwitchSceneCallback)
3. **保持交互状态** → 灯塔等交互对象保持激活状态（不重置）
4. **计算SAN奖励** → SaveManager.CalculateAndAwardReviveSan()
5. **显示复活面板** → 显示包含SAN奖励和游戏进度的复活面板
6. **显示复活数据** → SaveManager.SaveGameOnReviveAndShowData() (不重复计算SAN)
7. **完成复活** → 玩家在安全屋，可继续游戏

### 面板显示优化
- **死亡时**: 不显示任何面板，避免重复显示
- **复活时**: 显示统一的复活面板，包含所有相关信息
- **无奖励时**: 即使没有SAN奖励也显示复活面板，提供完整的游戏反馈

### 防重复机制
- PlayerController中先计算SAN奖励
- SaveManager的SaveGameOnReviveAndShowData()方法接收参数控制是否重复计算
- 确保每次复活只获得一次SAN奖励

### 交互对象状态保持
- **灯塔(LightHouse)**: 玩家死亡时不重置激活状态，保持用于SAN奖励计算
- **其他交互对象**: 同样保持状态，避免丢失探索进度
- **真正重置**: 只在清空游戏数据时才完全重置所有状态

## 测试组件

### 1. ReviveSanTester.cs
- **功能**: 键盘快捷键测试
- **位置**: `Assets/Scripts/Test/ReviveSanTester.cs`
- **按键说明**:
  - `F12`: 计算复活SAN奖励
  - `F11`: 清空复活SAN数据
  - `F10`: 添加测试交互对象
  - `F9`: 显示复活SAN详情
  - `F7`: **测试完整复活流程** (推荐)

### 2. ReviveSanTestConsole.cs
- **功能**: 可视化测试控制台UI
- **位置**: `Assets/Scripts/Test/ReviveSanTestConsole.cs`
- **按键说明**:
  - `F8`: 切换控制台显示/隐藏

### 6. DeathReviveFlowTest.cs
- **功能**: 死亡复活流程测试
- **位置**: `Assets/Scripts/Test/DeathReviveFlowTest.cs`
- **按键说明**:
  - `K键`: 普通死亡测试（验证不显示死亡面板）
  - `B键`: 战斗死亡测试
  - `M键`: SAN归零死亡测试（显示AVG面板）
  - `I键`: 显示当前玩家状态

## 使用方法

### 快速测试
1. 在任意场景中添加一个空GameObject
2. 将 `ReviveSanTestScene.cs` 脚本附加到该GameObject
3. 运行游戏，脚本会自动配置测试环境
4. 使用**F7键测试完整复活流程**（推荐）或F9-F12按键进行单独功能测试

### 真实复活测试
1. **正常游戏**: 在游戏中正常探索，激活交互对象
2. **触发死亡**: 让玩家HP降至0或触发死亡条件
3. **自动复活**: 系统会自动计算SAN奖励并显示复活面板
4. **验证奖励**: 检查SAN值是否正确增加

### 详细测试步骤

#### 步骤1: 环境准备
```csharp
// 自动执行（如果autoSetupOnStart = true）
// 或手动调用
ReviveSanTestScene.SetupTestEnvironment();
```

#### 步骤2: 模拟玩家交互
```csharp
// 方法1: 使用F10键添加单个测试交互
// 方法2: 调用生成随机交互
ReviveSanTester.GenerateRandomTestInteractions();
// 方法3: 激活现有交互对象
ReviveSanTestScene.ActivateTestInteractions();
```

#### 步骤3: 测试SAN奖励计算
```csharp
// 使用F12键或直接调用
SaveManager.Instance.CalculateAndAwardReviveSan();
```

#### 步骤4: 查看结果
```csharp
// 使用F9键显示详情
// 或查看控制台输出
```

### 测试场景

#### 场景1: 完整复活流程测试 (推荐)
1. 按F7键触发完整复活流程测试
2. 系统会自动：添加测试交互 → 计算SAN奖励 → 显示复活数据 → 显示奖励面板
3. 验证整个流程是否正常工作

#### 场景2: 基础SAN奖励测试
1. 激活几个灯塔（每个1 SAN）
2. 激活几个休息点（每个2 SAN）
3. 调用 `CalculateAndAwardReviveSan()`
4. 验证SAN值正确增加

#### 场景3: 重复复活测试
1. 执行场景1或场景2
2. 再次调用 `CalculateAndAwardReviveSan()`
3. 验证不会重复获得SAN奖励

#### 场景4: 真实游戏复活测试
1. 在游戏中正常探索并激活交互对象
2. 让玩家死亡触发复活
3. 验证复活时自动获得正确的SAN奖励
4. 检查复活面板是否正确显示

#### 场景5: 数据持久化测试
1. 激活交互对象并获得SAN奖励
2. 重启游戏
3. 验证数据正确恢复

#### 场景8: 死亡复活流程测试
1. 按K键测试普通死亡（验证不显示死亡面板）
2. 按B键测试战斗死亡流程
3. 按M键测试SAN归零死亡（应显示特殊AVG面板）
4. 验证只在复活时显示面板，包含完整信息

#### 场景9: 面板显示时机验证
1. 触发各种死亡情况
2. 验证死亡时不显示GameOverPanel
3. 验证复活时显示统一的复活面板
4. 确认面板包含SAN奖励和游戏进度信息

## SAN奖励表

### 基础交互对象
| 对象类型 | SAN奖励 | 对应代码变量 |
|---------|---------|-------------|
| 灯塔 | 1 | lightHouseIsDic |
| 休息点 | 2 | restPointDic |
| 关键点 | 1 | keyPointDic |
| 道具点 | 1 | itemPointDic |

### 特殊交互对象
| 对象名称 | SAN奖励 | 描述 |
|---------|---------|------|
| 假墙 | 3 | 隐藏区域发现 |
| 墙中鼠支线 | 2 | 支线任务完成 |
| 地下室钥匙 | 5 | 重要道具获得 |
| 扫把 | 1 | 普通道具 |
| 灯泡 | 1 | 普通道具 |
| ... | ... | 详见SaveManager.cs |

### Boss战胜利奖励
| Boss名称 | SAN奖励 | 变量名 |
|---------|---------|--------|
| 一楼Boss | 4 | isFirstFloorBossDefeated |
| 二楼Boss | 6 | isSecondFloorBossDefeated |
| 三楼Boss | 8 | isThirdFloorBossDefeated |
| 最终Boss | 100 | isFinalBossDefeated |

## 调试信息

### 控制台输出示例
```
=== 计算复活SAN奖励 ===
检查交互对象奖励...
灯塔奖励: 3个 * 1 SAN = 3 SAN
休息点奖励: 2个 * 2 SAN = 4 SAN
Boss战奖励: 1个 * 4 SAN = 4 SAN
总共获得SAN奖励: 11
当前玩家SAN: 61.0/100.0
复活SAN数据已保存
```

### 错误排查

#### 常见问题1: SaveManager.Instance 为空
**解决方案**: 确保场景中存在SaveManager或相关的单例管理器

#### 常见问题2: GameLevelManager.Instance 为空
**解决方案**: 确保场景中存在GameLevelManager组件

#### 常见问题3: PlayerManager.Instance.player 为空
**解决方案**: 确保玩家对象已正确初始化

#### 常见问题4: SAN奖励计算为0
**解决方案**: 
1. 检查是否有激活的交互对象
2. 检查是否已经获得过奖励（防重复机制）
3. 使用F11清空数据后重试

## 性能注意事项

1. **内存使用**: 复活SAN数据使用JSON序列化，数据量适中
2. **存储位置**: 数据保存在 `Application.persistentDataPath + "/revive_san_data.json"`
3. **计算复杂度**: O(n) where n = 交互对象数量，性能良好

## 扩展功能

### 添加新的SAN奖励类型
1. 在SaveManager.cs的 `interactionRewardTable` 中添加映射
2. 在 `GetAllInteractionIds()` 中添加数据收集逻辑
3. 更新测试脚本以包含新类型

### 自定义奖励计算规则
可以修改 `CalculateAndAwardReviveSan()` 方法来实现：
- 基于难度的奖励倍率
- 基于时间的奖励衰减
- 基于完成度的额外奖励

## 测试检查清单

- [ ] **完整复活流程测试** (F7键)
- [ ] **死亡时不显示面板** (K键测试)
- [ ] **复活时显示完整面板** (包含SAN奖励和进度)
- [ ] 基础SAN奖励计算正确
- [ ] 防重复奖励机制工作
- [ ] 复活面板正确显示
- [ ] 数据持久化保存/加载
- [ ] 清空数据功能正常
- [ ] UI控制台显示正确
- [ ] 键盘快捷键响应
- [ ] Boss战奖励计算
- [ ] 特殊交互对象奖励
- [ ] 真实游戏复活测试
- [ ] 错误处理和异常情况
- [ ] 性能表现可接受

---

**注意**: 这是测试系统，仅用于开发和调试。在正式版本中可以移除Test文件夹下的所有测试脚本。
