### Unity工程引入`Protobuf`源码解决方案

1. Download [Google.Protobuf](https://github.com/protocolbuffers/protobuf/tree/main/csharp/src/Google.Protobuf) .
2. create new unity project , (Google.Protobuf ) Copy to the Assets folder .
3. unity enable (Player Setting ->Other Setting -> allow 'unsafe' Code)
4. unity console show more CS0122 error
   ```shell
   Assets\ThirdParty\Google.Protobuf\WritingPrimitives.cs(281,21): error CS0103: The name 'Unsafe' does not exist in the current context
   ```
5. error 解决方案

   [error CS0122: 'Unsafe' is inaccessible due to its protection level](https://github.com/protocolbuffers/protobuf/issues/10085)

   ```shell
   Unity also provides extra "System.Runtime.CompilerServices.Unsafe.dll" inside their Editor install folder.
   For example in my case:
   "C:\Program Files\Unity\Hub\Editor\2022.1.15f1\Editor\Data\MonoBleedingEdge\lib\mono\msbuild\Current\bin\Roslyn\System.Runtime.CompilerServices.Unsafe.dll"
   
   Copy this file and let it sits inside your Unity project folder "Assets/Plugins"
   And Unity will redirect project compilation to use this instead.
   The same can be done if another .NET DLL dependency is requested, like "System.Buffers.dll" or "System.Memory.dll"
   ```
### RecyclableMemoryStream
* 引入 Microsoft.IO

### TextMesh Pro
* TextMesh Pro UI 组件

### 协议生成工具接入使用说明
1. 