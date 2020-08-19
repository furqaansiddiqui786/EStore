using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace FlipShop_OnlineShopping.utilities
{
    public static class SD
    {
        public const string DefaultImage = "noimg.png";

        public const string SuperAdminUser = "SuperAdmin";

        public const string AdminUser = "Admin";

        public const string CustomerUser = "Customer";

        public const string SiteManagers = SuperAdminUser + " , " + AdminUser;


        public static string ConvertToRawHtml(string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }

        public const string StatusSubmitted = "Submitted";
        public const string StatusInProgress = "Being prepared";
        public const string StatusReady = "Status Ready";
        public const string StatusCompleted = "Completed";
        public const string StatusCancelled = "Cancelled";

        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusApproved = "Approved";
        public const string PaymentStatusRejected = "Rejected";
    }
}
