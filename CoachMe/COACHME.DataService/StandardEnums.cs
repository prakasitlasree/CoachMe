﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COACHME.DATASERVICE
{
    public static class StandardEnums
    {
        public enum ConfigurationSettingName
        {
            MAIL_SENDER,
            MAIL_SUBJECT,
            MAIL_BODY,
            MAIL_FOOTER,
            SMTP_ACCOUNT,
            SMTP_PASSWORD,
            FORGET_PASSWORD_URL,
            MAIL_SUBJECT_REGISTER,
            MAIL_BODY_REGISTER,
            MAIL_FOOTER_REGISTER
        }

        public enum SurveyType
        {
            Interest 
        }

        public enum ActivatePurchaseStatus
        {
            DRAFT,
            ACTIVE

        }
    }
}
