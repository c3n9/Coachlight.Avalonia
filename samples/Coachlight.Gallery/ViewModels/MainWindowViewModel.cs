using Avalonia;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Coachlight.Avalonia;              // StartTour extension
using Coachlight.Avalonia.Building;
using Coachlight.Avalonia.Enums;

namespace Coachlight.Gallery.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    /// <summary>
    /// Собирает демо-тур и запускает его поверх окна. Якорь (окно) приходит как
    /// CommandParameter — так VM не держит прямую ссылку на View.
    /// </summary>
    [RelayCommand]
    private void StartTour(Visual? anchor)
    {
        if (anchor is null)
            return;

        var tour = TourBuilder.Create("demo")
            .Modal(s => s
                .Title("Привет!")
                .Text("Это короткий тур по интерфейсу. Нажимай «Next»."))
            .Coachmark("btnConnect", s => s
                .Placement(Side.Right)
                .Title("Подключение")
                .Text("Эта кнопка соединяет с роботом."))
            .Coachmark("btnSettings", s => s
                .Placement(Side.Bottom)
                .Title("Настройки")
                .Text("А здесь настройки приложения."))
            .Build();

        anchor.StartTour(tour);
    }
}
