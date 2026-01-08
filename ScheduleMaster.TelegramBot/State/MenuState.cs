namespace ScheduleMaster.TelegramBot.States
{
    public class MenuState
    {
        public long ChatId { get; set; }
        public MenuStep CurrentStep { get; set; } = MenuStep.MainMenu;
        public StudioMenuStep? StudioStep { get; set; }
        public int? SelectedStudioId { get; set; }
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
        JoinStudio,
        MyStudios,
        MyStudiosDetail,
        StudiosIAmIn,
    }
}
