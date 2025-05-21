using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AjudeiMais.Ecommerce.Filters
{
    public class RoleAuthorizeAttribute : ActionFilterAttribute
    {
        private readonly string[] _roles;

        public RoleAuthorizeAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var roleAtual = context.HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(roleAtual) || !_roles.Contains(roleAtual.ToLower()))
            {
                context.Result = new RedirectToActionResult("AcessoNegado", "Home", null);
            }
        }

    }
}
