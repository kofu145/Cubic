namespace Cubic.Graphics;

/// <summary>
/// Represents a shader attachment that is used during shader initialization.
/// </summary>
public struct ShaderAttachment
{
    /// <summary>
    /// The type of attachment.
    /// </summary>
    public readonly AttachmentType Attachment;
    
    /// <summary>
    /// The code that will be used for this attachment.
    /// </summary>
    public readonly string Code;
    
    /// <summary>
    /// RESERVED: DO NOT EDIT.
    /// </summary>
    public uint TempHandle;

    /// <summary>
    /// Create a new shader attachment with the given attachment type and code.
    /// </summary>
    /// <param name="attachment">The attachment type.</param>
    /// <param name="code">The code to use.</param>
    public ShaderAttachment(AttachmentType attachment, string code)
    {
        Attachment = attachment;
        Code = code;
        TempHandle = 0;
    }
}