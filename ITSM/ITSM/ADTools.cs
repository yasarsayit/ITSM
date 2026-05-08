using System;
using System.DirectoryServices.AccountManagement;

namespace ITSM
{
    public class ADTools
    {
        private readonly string _domain = "";

        public string[] GetUserInfo(string userName)
        {
            try
            {
                using (PrincipalContext dc = new PrincipalContext(ContextType.Domain, _domain))
                {
                    UserPrincipal user = UserPrincipal.FindByIdentity(dc, userName);

                    if (user != null)
                    {
                        return new string[]
                        {
                            user.EmailAddress ?? "",
                            user.DisplayName ?? "",
                            user.Description ?? "" // sicil no
                        };
                    }
                }
            }
            catch
            {
            }

            return new string[] { "", "", "" };
        }

        public string GetEmail(string userName)
        {
            var info = GetUserInfo(userName);
            return info[0];
        }

        public string GetName(string userName)
        {
            var info = GetUserInfo(userName);
            return info[1];
        }

        public string GetSicilNo(string userName)
        {
            var info = GetUserInfo(userName);
            return info[2];
        }
    }
}