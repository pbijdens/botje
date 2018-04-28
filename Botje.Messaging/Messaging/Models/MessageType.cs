namespace Botje.Messaging.Models
{
    /// <summary>
    /// Enum created for convenience.
    /// </summary>
    public enum MessageType
    {
        Default = 0,
        Audio = 1,
        Video = 2,
        VideoNote = 3,
        Voice = 4,
        Document = 5,
        Location = 6,
        Sticker = 7,
        TextMessage = 8,
        NewChatMembers = 9,
        LeftChatMember = 10,
        MigrateTo = 11,
        MigrateFrom = 12,
        PinnedMessage = 13,
        DeleteChatPhoto = 14,
        NewChatTitle = 15,
        NewChatPhoto = 16,
        ConnectedWebsite = 17,
        GroupChatCreated = 18,
        SupergroupChatCreated = 19,
        ChannelChatCreated = 20
    }
}