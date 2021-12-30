﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Quiz.App.Extensions
{
    public static class ModelStateExtensions
    {
        public static void AddIdentityErrors(this ModelStateDictionary modelState, IEnumerable<IdentityError> identityErrors)
        {
            foreach (var error in identityErrors)
            {
                modelState.AddModelError(error.Code, error.Description);
            }
        }
        
    }
}