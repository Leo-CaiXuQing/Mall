# Mall
一个简单的C#微服务代码,包含使用Consul,Ocelot,Polly等
1.请先运行Consul  ，否则无法注册发现API命令： consul.exe agent -dev
Consul本地地址演示：http://localhost:8500/ui/dc1/services

2.启动API项目，请使用控制台命令执行以下代码，否则Api项目会报错    --ip="127.0.0.1"(ip地址)  --port="12347"(端口号) --weight=2 (权重)
dotNet Mall.WebApi.dll --urls="http://*:12347" --ip="127.0.0.1"  --port="12347" --weight=2

3.启动Gateway项目  
dotnet Mall.Gateway.dll --urls="http://*:12321" --ip="127.0.0.1" --port="12321"


4.最后启动Mall
dotnet Mall.dll --urls="http://*:123451" --ip="127.0.0.1" --port="12345"
