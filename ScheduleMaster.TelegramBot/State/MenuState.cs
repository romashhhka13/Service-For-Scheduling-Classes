namespace ScheduleMaster.TelegramBot.States
{
    public class MenuState
    {
        public long ChatId { get; set; }
        public Guid? SelectedUserId { get; set; }
        public Guid? SelectedStudioCategoryId { get; set; }
        public string? PendingStudioName { get; set; }

        public MenuStep CurrentStep { get; set; } = MenuStep.MainMenu;
        public StudioMenuStep? StudioStep { get; set; }
        public Guid? SelectedStudioId { get; set; }

    }

    public enum MenuStep
    {
        MainMenu,
        Profile,
        Studios,
        Calendar,
    }

    public enum StudioMenuStep
    {
        StudioMainMenu,
        CreateStudio,
        CreateStudioTitle,

        CreateStudioCategory,
        JoinStudio,
        MyStudios,
        MyStudiosDetail,
        StudiosIAmIn
    }
}
