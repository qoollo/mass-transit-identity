# MassTransit Identity
Library for actor identification through services in MassTransit. Repository also contains simple example projects with providers and consumers.
# Usage steps: 
## 1. Add nessesary services to service collection in your project
### 1.1 Add Identity Token to service collection 
Put this code in your ConfigureServices method:
```csharp
services.AddScoped<MassTransitIdentityToken>();
 ```
### 1.2 Add filter configurations to your MassTransit transport configuration
Add these lines:
```csharp
cfg.UsePublishFilter(typeof(IdentityTokenPublishFilter<>), context);
cfg.UseSendFilter(typeof(IdentityTokenSendFilter<>), context);
cfg.UseConsumeFilter(typeof(IdentityTokenConsumeFilter<>), context);
```
**WARNING:** It is strongly recommended to add all filters to configuration, no matter for what purposes you are going to use Identity Token: receiving, sending/publishing or redirecting. 
### 1.3 Put token as dependency where you need to
```csharp
class Service
{
    private readonly MassTransitIdentityToken _token;
    private readonly ILogger<Service> _logger;

    public Service(MassTransitIdentityToken token, 
            ILogger<Service> logger)
    {
        _token = token;
        _logger = logger;
    }
}
``` 
TIP: Identity token secured from rewriting.