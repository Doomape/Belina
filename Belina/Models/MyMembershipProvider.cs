using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Belina.Models;

public class MyMembershipProvider : MembershipProvider
{
    BelinaEntities2 db = new BelinaEntities2();

    public override string ApplicationName
    {
        get
        {
            throw new NotImplementedException();
        }
        set
        {
            throw new NotImplementedException();
        }
    }

    public override bool ChangePassword(string username, string oldPassword, string newPassword)
    {
        throw new NotImplementedException();
    }

    public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
    {
        throw new NotImplementedException();
    }

    public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
    {
        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] bs = System.Text.Encoding.UTF8.GetBytes(password);

        bs = md5.ComputeHash(bs);
        try
        {
            int exist = (from x in db.Administrator where x.user_name == username select x).Count();

            if (exist == 1)
            {
                //The user exists with that username and password
                status = MembershipCreateStatus.DuplicateUserName;
                return null;
            }

            object[] param = (object[])providerUserKey;

            Administrator admin = new Administrator();
            admin.user_name = username;
            admin.user_pass = bs;
            admin.user_email = (string)param[0];
            db.Administrator.Add(admin);
            db.SaveChanges();

            status = MembershipCreateStatus.Success;
            return GetUser(admin.user_name, true);

        }
        catch
        {
            status = MembershipCreateStatus.ProviderError;
            return null;
        }
        //throw new NotImplementedException();
    }

    public override bool DeleteUser(string username, bool deleteAllRelatedData)
    {
        throw new NotImplementedException();
    }

    public override bool EnablePasswordReset
    {
        get { throw new NotImplementedException(); }
    }

    public override bool EnablePasswordRetrieval
    {
        get { throw new NotImplementedException(); }
    }

    public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
    {
        throw new NotImplementedException();
    }

    public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
    {
        throw new NotImplementedException();
    }

    public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
    {
        throw new NotImplementedException();
    }

    public override int GetNumberOfUsersOnline()
    {
        throw new NotImplementedException();
    }

    public override string GetPassword(string username, string answer)
    {
        throw new NotImplementedException();
    }

    public override MembershipUser GetUser(string username, bool userIsOnline)
    {
        var result = from u in db.Administrator where (u.user_name == username) select u;

        if (result.Count() != 0)
        {
            var admin_dbuser = result.FirstOrDefault();

            string _username = admin_dbuser.user_name;
            int _providerUserKey = admin_dbuser.user_id;
            string _email = "";
            string _passwordQuestion = "";
            string _comment = "";
            bool _isApproved = true;
            bool _isLockedOut = false;
            DateTime _creationDate = DateTime.Now;
            DateTime _lastLoginDate = DateTime.Now;
            try
            {
                _lastLoginDate = DateTime.Now;
            }
            catch { }
            DateTime _lastActivityDate = DateTime.Now;
            DateTime _lastPasswordChangedDate = DateTime.Now;
            DateTime _lastLockedOutDate = DateTime.Now;

            MembershipUser admin = new MembershipUser("CustomMembershipProvider",
                                                      _username,
                                                      _providerUserKey,
                                                      _email,
                                                      _passwordQuestion,
                                                      _comment,
                                                      _isApproved,
                                                      _isLockedOut,
                                                      _creationDate,
                                                      _lastLoginDate,
                                                      _lastActivityDate,
                                                      _lastPasswordChangedDate,
                                                      _lastLockedOutDate);

            return admin;
        }
        else
        {
            return null;
        }
    }

    public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
    {
        throw new NotImplementedException();
    }

    public override string GetUserNameByEmail(string email)
    {
        throw new NotImplementedException();
    }

    public override int MaxInvalidPasswordAttempts
    {
        get { throw new NotImplementedException(); }
    }

    public override int MinRequiredNonAlphanumericCharacters
    {
        get { throw new NotImplementedException(); }
    }

    public override int MinRequiredPasswordLength
    {
        get { throw new NotImplementedException(); }
    }

    public override int PasswordAttemptWindow
    {
        get { throw new NotImplementedException(); }
    }

    public override MembershipPasswordFormat PasswordFormat
    {
        get { throw new NotImplementedException(); }
    }

    public override string PasswordStrengthRegularExpression
    {
        get { throw new NotImplementedException(); }
    }

    public override bool RequiresQuestionAndAnswer
    {
        get { throw new NotImplementedException(); }
    }

    public override bool RequiresUniqueEmail
    {
        get { throw new NotImplementedException(); }
    }

    public override string ResetPassword(string username, string answer)
    {
        throw new NotImplementedException();
    }

    public override bool UnlockUser(string userName)
    {
        throw new NotImplementedException();
    }

    public override void UpdateUser(MembershipUser user)
    {
        throw new NotImplementedException();
    }

    public override bool ValidateUser(string username, string password)
    {
        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] bs = System.Text.Encoding.UTF8.GetBytes(password);

        bs = md5.ComputeHash(bs);
        try
        {
            var user = (from x in db.Administrator where x.user_name == username && x.user_pass == bs select x);

            if (user.Count() == 1)
            {
                return true;
            }
        }
        catch
        {
        }

        return false;
    }
}
