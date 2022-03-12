# MassTransit Identity
Library for actor identification through services in MassTransit. Repository also contains simple example projects with providers and consumers.
# Usage steps: 
## 1. Add nessesary services to service collection in your project
### **1.1 Add Identity Token to service collection**
Put this code in your ConfigureServices method:
```csharp
services.AddScoped<MassTransitIdentityToken>();
 ```
### **1.2 Add filter configurations to your MassTransit transport configuration**
Add these lines:
```csharp
cfg.UsePublishFilter(typeof(IdentityTokenPublishFilter<>), context);
cfg.UseSendFilter(typeof(IdentityTokenSendFilter<>), context);
cfg.UseConsumeFilter(typeof(IdentityTokenConsumeFilter<>), context);
```
**WARNING:** It is strongly recommended to add all filters to configuration, no matter for what purposes you are going to use Identity Token: receiving, sending/publishing or redirecting. 
## 2. Use token where you need to
### **2.1 Put token as dependency in service constructor**
```csharp
public class ScopedService
{
    private readonly MassTransitIdentityToken _token;
    private readonly ILogger<ScopedService> _logger;
    private readonly IRequestClient<SomeMassTransitRequest> _requestClient;

    public ScopedService(MassTransitIdentityToken token, 
            ILogger<ScopedService> logger,
            IRequestClient<MassTransitContract> requestClient)
    {
        _token = token;
        _logger = logger;
        _requestClient = requestClient;
    }
}
``` 
### **2.2 Create identity token for your scope**
Put token into ```Value``` property of MassTransitIdentityToken instance. After this you can call services, which use identity token. What is more important, now you can send and publish messages to other MassTransit actors like consumers.
```csharp
public class ScopedService
{
    ...

    public void Foo(Guid clientId)
    {
        _token.Value = clientId;

        var response = _requestClient.GetResponse<SomeMassTansitResponse>(new { });
    }
}
``` 
**TIP**: As you can see, identity token value type is ```Guid```.
### **2.3 Get identity token in other services**
```csharp
public class AnotherServiceConsumer : IConsumer<SomeMassTransitRequest>
{
    private readonly MassTransitIdentityToken _token;
    private readonly ILogger<AnotherServiceConsumer> _logger;

    public SomeConsumer(MassTransitIdentityToken token, 
        ILogger<AnotherServiceConsumer> logger)
    {
        _token = token;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SomeMassTransitRequest> context)
    {
        _logger.LogInformation($"{typeof(AnotherServiceConsumer)}: Consumed token {_token.Value}");

        await context.RespondAsync<SomeMassTansitResponse>(new { });
    }
}
```
**TIP**: Identity token secured from rewriting.
