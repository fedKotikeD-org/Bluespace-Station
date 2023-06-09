using Content.Client.Cargo.UI;
using Content.Shared.Cargo.BUI;
using Content.Shared.Cargo.Events;
using Robust.Client.GameObjects;

namespace Content.Client.Cargo.BUI;

public sealed class CargoPalletConsoleBoundUserInterface : BoundUserInterface
{
    private CargoPalletMenu? _menu;

    public CargoPalletConsoleBoundUserInterface(ClientUserInterfaceComponent owner, Enum uiKey) : base(owner, uiKey) {}

    protected override void Open()
    {
        base.Open();

        _menu = new CargoPalletMenu();
        _menu.AppraiseRequested += OnAppraisal;
        _menu.SellRequested += OnSell;
        _menu.OnClose += Close;

        _menu.OpenCentered();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _menu?.Dispose();
        }
    }

    private void OnAppraisal()
    {
        SendMessage(new CargoPalletAppraiseMessage());
    }

    private void OnSell()
    {
        SendMessage(new CargoPalletSellMessage());
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is not CargoPalletConsoleInterfaceState palletState)
            return;

        _menu?.SetEnabled(palletState.Enabled);
        _menu?.SetAppraisal(palletState.Appraisal);
        _menu?.SetCount(palletState.Count);
    }
}
