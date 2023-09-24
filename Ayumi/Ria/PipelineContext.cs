using Arvy;

namespace Ria;

public class PipelineContext {
    public IList<ActionResponseViewModel> ActionMessages { get; set; } = new List<ActionResponseViewModel>();
    public IDictionary<String, Object> Data { get; set; } = new Dictionary<String, Object>();
    public Boolean Cancelled { get; set; }
}