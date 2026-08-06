using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManagementMaui.Shared.Models.Common
{
    public class ApiErrorResponse
    {
        public string? Message { get; set; }

        public string? Title { get; set; }

        public Dictionary<string, string[]>? Errors { get; set; }

        public string GetErrorMessage()
        {
            if (!string.IsNullOrWhiteSpace(Message))
            {
                return Message;
            }

            if (Errors is not null && Errors.Count > 0)
            {
                var messages = Errors.Values
                    .SelectMany(x => x)
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct()
                    .ToList();

                if (messages.Count > 0)
                {
                    return string.Join(" ", messages);
                }
            }

            if (!string.IsNullOrWhiteSpace(Title))
            {
                return Title;
            }

            return "İşlem sırasında bir hata oluştu. Lütfen tekrar deneyiniz.";
        }
    }
}
