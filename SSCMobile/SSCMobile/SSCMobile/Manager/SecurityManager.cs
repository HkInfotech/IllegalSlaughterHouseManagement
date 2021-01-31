using System;
using System.Collections.Generic;
using System.Text;

namespace SSCMobile.Manager
{
    public enum SecurityPermission
    {
        Read,
        ReadWrite,
        Delete,
        NoAccess,
        Full
    }


    public class SecurityManager
    {
        public SecurityPermission GetSecurityPermission(string permissionLabel)
        {
            SecurityPermission perm = SecurityPermission.Read;
            if (!string.IsNullOrWhiteSpace(permissionLabel))
            {
                string value = permissionLabel.ToUpper();

                if (permissionLabel.Contains("OPEN") || permissionLabel.Contains("NEW"))
                {
                    perm = SecurityPermission.ReadWrite;
                }

                if (value.Contains("CRUD"))
                {
                    return SecurityPermission.Full;
                }

                if (value.Contains("C"))
                {
                    return SecurityPermission.ReadWrite;
                }

                if (value.Contains("R"))
                {
                    return SecurityPermission.Read;
                }

                if (value.Contains("U"))
                {
                    return SecurityPermission.ReadWrite;
                }

                if (value.Contains("D"))
                {
                    return SecurityPermission.Delete;
                }
            }

            return perm;
        }
    }
}
