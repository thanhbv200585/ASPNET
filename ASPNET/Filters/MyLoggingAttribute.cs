using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ASPNET.Filter
{
    public class MyLoggingAttribute : Attribute, IActionFilter
    {
        private readonly string _callerName;

        public MyLoggingAttribute(string callerName)
        {
            _callerName = callerName;
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
            Console.WriteLine($"Filter executed After {_callerName}");
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            Console.WriteLine($"Filter executed Before {_callerName}");
        }
    }
}
