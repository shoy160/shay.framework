﻿using Shay.Core.Exceptions;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shay.Payment.Helper
{
    public static class ValidateHelper
    {
        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="obj">验证目标</param>
        /// <param name="data">上下文数据</param>
        public static void Validate(object obj, Dictionary<object, object> data)
        {
            var validationContext = new ValidationContext(obj, data);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(obj, validationContext, results, true);

            if (!isValid)
            {
                throw new BusiException(results[0].ErrorMessage, 20001);
            }
        }
    }
}
