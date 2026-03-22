namespace SharedKernel.Constants;

/// <summary>
/// Константы с кодами ошибок для доменных исключений
/// </summary>
public static class DomainErrors
{
    public static class Money
    {
        public const string InvalidAmount = "money.invalid_amount";
    }

    public static class PlanName
    {
        public const string Empty = "plan_name.empty";
        public const string TooLong = "plan_name.too_long";
    }

    public static class UserEmail
    {
        public const string Empty = "user.email.empty";
        public const string Invalid = "user.email.invalid";
    }

    public static class Plan
    {
        public const string InvalidId = "plan.invalid_id";
        public const string NotFound = "plan.not_found";
        public const string NotActive = "plan.not_active";
        public const string AlreadyActive = "plan.already_active";
        public const string AlreadyDeactivated = "plan.already_deactivated";
    }

    public static class User
    {
        public const string InvalidId = "user.invalid_id";
        public const string NotFound = "user.not_found";
        public const string TrialAlreadyUsed = "user.trial_already_used";
        public const string EmailAlreadyExists = "user.email_already_exists";
    }

    public static class Invoice
    {
        public const string InvalidId = "invoice.invalid_id";
        public const string AlreadyPaid = "invoice.already_paid";
        public const string AlreadyFailed = "invoice.already_failed";
        public const string NotFound = "invoice.not_found";
    }

    public static class Subscription
    {
        public const string InvalidId = "subscription.invalid_id";
        public const string InvalidUserId = "subscription.invalid_user_id";
        public const string InvalidPlanId = "subscription.invalid_plan_id";
        public const string NotFound = "subscription.not_found";
        public const string AlreadyCancelled = "subscription.already_cancelled";
        public const string AlreadyExpired = "subscription.already_expired";
        public const string CannotChangePlan = "subscription.cannot_change_plan";
        public const string CannotRenew = "subscription.cannot_renew";
    }
}