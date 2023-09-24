using Arvy;

namespace Valerie;

public class Validated<TValidated> {
    public Boolean ContainsFail { get; set; }
    public Boolean AllFails { get; set; }
    public IList<ActionResponseViewModel> Messages { get; set; } = new List<ActionResponseViewModel>();
    public IList<TValidated> ValidData { get; set; } = new List<TValidated>();
}