namespace Cubic.Graphics;

public struct ShaderAttachment
{
    public AttachmentType Attachment;
    public string Code;
    public uint TempHandle;

    public ShaderAttachment(AttachmentType attachment, string code)
    {
        Attachment = attachment;
        Code = code;
        TempHandle = 0;
    }
}