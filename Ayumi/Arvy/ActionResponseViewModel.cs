using Exy;

namespace Arvy;

public record ActionResponseViewModel(String ResponseType, String Message) {
    public const String Info = "I";
    public const String Warning = "W";
    public const String Error = "E";
    public const String Success = "S";

    public override String ToString() => ToString(true);

    public String ToString(Boolean alwaysReturn) {
        if (!alwaysReturn && ResponseType == Error)
            throw new UnintendedBehaviorException(Message);

        return ResponseType + "|" + Message;
    }
};