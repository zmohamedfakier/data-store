//using Microsoft.AspNetCore.Http;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Security.Principal;
//using System.Threading;
//using System.Threading.Tasks;

//namespace DataStore
//{
//  public class AnonymousAuthenticationFilter : IAuthenticationFilter
//  {
//    public bool AllowMultiple => true;

//    public Task AuthenticateAsync(HttpAuthenticationContext context, CancellationToken cancellationToken)
//    {
//      SetPrincipal(null);
//      return Task.CompletedTask;
//    }

//    public Task ChallengeAsync(HttpAuthenticationChallengeContext context, CancellationToken cancellationToken)
//    {
//      return Task.CompletedTask;
//    }

//    private void SetPrincipal(IPrincipal principal)
//    {
//      Thread.CurrentPrincipal = principal;
//    }
//  }
//}
