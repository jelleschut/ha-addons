using MudBlazor;

namespace HAShoppingList.Web.Extensions;

public static class DialogServiceExtensions
{
    public static async Task<bool> ConfirmDeleteAsync(this IDialogService dialogService, string name)
    {
        var parameters = new DialogParameters<MudMessageBox>
        {
            { x => x.Message,    $"Weet je zeker dat je '{name}' wilt verwijderen?" },
            { x => x.YesText,    "Verwijderen" },
            { x => x.CancelText, "Annuleren" }
        };
        var dialog = await dialogService.ShowAsync<MudMessageBox>("Verwijderen", parameters);
        var result = await dialog.Result;
        return result is not { Canceled: true };
    }
}
