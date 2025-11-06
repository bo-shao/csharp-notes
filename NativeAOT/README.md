# NativeAOT Console Application

这是一个使用 NativeAOT 编译的 C# 控制台应用程序。

## 什么是 NativeAOT？

NativeAOT (Ahead-of-Time) 是 .NET 的一种编译方式，它将 C# 代码直接编译为原生机器码，而不是 IL (Intermediate Language)。

### 优点：
- **快速启动**: 不需要 JIT 编译，应用启动更快
- **小体积**: 可以生成独立的单文件可执行文件，不需要 .NET 运行时
- **低内存占用**: 相比传统 .NET 应用，内存占用更少
- **原生性能**: 直接运行原生机器码

### 限制：
- 不支持动态代码生成（如 `Reflection.Emit`）
- 某些反射功能受限
- 编译时间较长

## 如何构建

### 开发模式（普通运行）
```bash
dotnet run
```

### 发布为 NativeAOT
```bash
dotnet publish -c Release
```

### 发布为单文件可执行文件
```bash
dotnet publish -c Release -r win-x64
```

或针对其他平台：
- Linux: `-r linux-x64`
- macOS: `-r osx-x64` 或 `-r osx-arm64`

## 项目配置说明

在 `.csproj` 文件中的关键配置：

- `<PublishAot>true</PublishAot>`: 启用 NativeAOT 编译
- `<IlcOptimizationPreference>Size</IlcOptimizationPreference>`: 优化体积
- `<PublishTrimmed>true</PublishTrimmed>`: 启用裁剪，移除未使用的代码
- `<TrimMode>full</TrimMode>`: 完全裁剪模式

## 运行

发布后，可执行文件位于：
```
bin\Release\net8.0\win-x64\publish\AotConsoleApp.exe
```

直接运行该可执行文件即可，无需安装 .NET 运行时。
