using Botje.Messaging.Models;
using System.Collections.Generic;
using Xunit;

namespace Botje.Messaging.Test
{
    /// <summary>
    /// Test code to test basic deserialization. Used to check if the data model and serialization attribute mappings keep working when changes to the models are made.
    /// </summary>
    public class DeserializerTests
    {
        private RestSharp.Deserializers.JsonDeserializer _rsDeserializer = new RestSharp.Deserializers.JsonDeserializer();

        [Fact]
        public void ChatTitleChangedMessagesCanBeParsed()
        {
            string strGetUpdatesResult = "{\"ok\":true,\"result\":[{\"update_id\":519015210,\n\"message\":{\"message_id\":2532,\"from\":{\"id\":195152195,\"is_bot\":false,\"first_name\":\"Pieter-Bas\",\"username\":\"PieterBass\",\"language_code\":\"en-US\"},\"chat\":{\"id\":-280623243,\"title\":\"Onno Test Supergroup (With new title)\",\"type\":\"group\",\"all_members_are_administrators\":true},\"date\":1524901470,\"new_chat_title\":\"Onno Test Supergroup (With new title)\"}}]}";
            var objGetUpdatesResult = _rsDeserializer.Deserialize<Result<List<GetUpdatesResult>>>(new RestSharp.RestResponse { Content = strGetUpdatesResult });

            Assert.Collection(objGetUpdatesResult.Data,
                (x) => Assert.Equal("Onno Test Supergroup (With new title)", x.Message.NewChatTitle)
                );
        }

        [Fact]
        public void ChatPhotoChangedMessagesCanBeparsed()
        {
            string strGetUpdatesResult = "{\"ok\":true,\"result\":[{\"update_id\":519015209,\"message\":{\"message_id\":2530,\"from\":{\"id\":195152195,\"is_bot\":false,\"first_name\":\"Pieter-Bas\",\"username\":\"PieterBass\",\"language_code\":\"en-US\"},\"chat\":{\"id\":-280623243,\"title\":\"Onno Test Supergroup\",\"type\":\"group\",\"all_members_are_administrators\":true},\"date\":1524826809,\"new_chat_photo\":[{\"file_id\":\"AgADBAAD0acxG0PJoQsEp2GSrg6EclMbnxoABPUn_6TiSN5EJzwBAAEC\",\"file_size\":7405,\"width\":160,\"height\":160},{\"file_id\":\"AgADBAAD0acxG0PJoQsEp2GSrg6EclMbnxoABK5KUQHU31-5KDwBAAEC\",\"file_size\":17136,\"width\":320,\"height\":320},{\"file_id\":\"AgADBAAD0acxG0PJoQsEp2GSrg6EclMbnxoABJBKOcGaZ2LYKTwBAAEC\",\"file_size\":40568,\"width\":640,\"height\":640}]}}]}";
            var objGetUpdatesResult = _rsDeserializer.Deserialize<Result<List<GetUpdatesResult>>>(new RestSharp.RestResponse { Content = strGetUpdatesResult });
            Assert.Collection(objGetUpdatesResult.Data,
                (x) => Assert.Collection(x.Message.NewChatPhoto,
                            (y) => Assert.Equal("1601607405AgADBAAD0acxG0PJoQsEp2GSrg6EclMbnxoABPUn_6TiSN5EJzwBAAEC", $"{y.Width}{y.Height}{y.FileSize}{y.FileID}"),
                            (y) => Assert.Equal("32032017136AgADBAAD0acxG0PJoQsEp2GSrg6EclMbnxoABK5KUQHU31-5KDwBAAEC", $"{y.Width}{y.Height}{y.FileSize}{y.FileID}"),
                            (y) => Assert.Equal("64064040568AgADBAAD0acxG0PJoQsEp2GSrg6EclMbnxoABJBKOcGaZ2LYKTwBAAEC", $"{y.Width}{y.Height}{y.FileSize}{y.FileID}")
                       )
                );
        }

        [Fact]
        public void TextMessagesCanBeParsed()
        {
            string strGetUpdatesResult = "{\"ok\":true,\"result\":[{\"update_id\":519015211,\n\"message\":{\"message_id\":2534,\"from\":{\"id\":195152195,\"is_bot\":false,\"first_name\":\"Pieter-Bas\",\"username\":\"PieterBass\",\"language_code\":\"en-US\"},\"chat\":{\"id\":-280623243,\"title\":\"Onno Test Supergroup (With new title)\",\"type\":\"group\",\"all_members_are_administrators\":true},\"date\":1524902320,\"text\":\"Text message\"}}]}";
            var objGetUpdatesResult = _rsDeserializer.Deserialize<Result<List<GetUpdatesResult>>>(new RestSharp.RestResponse { Content = strGetUpdatesResult });
            Assert.Collection(objGetUpdatesResult.Data,
                (x) => Assert.Equal("Text message", x.Message.Text)
                );
        }

        [Fact]
        public void UpgradeToSupergroupMessagesCanBeParsed()
        {
            string strGetUpdatesResult = "{\"ok\":true,\"result\":[{\"update_id\":519015212,\n\"message\":{\"message_id\":2536,\"from\":{\"id\":195152195,\"is_bot\":false,\"first_name\":\"Pieter-Bas\",\"username\":\"PieterBass\",\"language_code\":\"en-US\"},\"chat\":{\"id\":-280623243,\"title\":\"Onno Test Supergroup (With new title)\",\"type\":\"group\",\"all_members_are_administrators\":true},\"date\":1524902838,\"migrate_to_chat_id\":-1001255131662}},{\"update_id\":519015213,\n\"message\":{\"message_id\":1,\"from\":{\"id\":195152195,\"is_bot\":false,\"first_name\":\"Pieter-Bas\",\"username\":\"PieterBass\",\"language_code\":\"en-US\"},\"chat\":{\"id\":-1001255131662,\"title\":\"Onno Test Supergroup (With new title)\",\"type\":\"supergroup\"},\"date\":1524902838,\"migrate_from_chat_id\":-280623243}}]}";
            var objGetUpdatesResult = _rsDeserializer.Deserialize<Result<List<GetUpdatesResult>>>(new RestSharp.RestResponse { Content = strGetUpdatesResult });
            Assert.Collection(objGetUpdatesResult.Data,
                (x) =>
                {
                    Assert.Equal(-280623243, x.Message.Chat.ID);
                    Assert.Equal(-1001255131662, x.Message.MigrateToChatID.Value);
                },
                (x) =>
                    {
                        Assert.Equal(-1001255131662, x.Message.Chat.ID);
                        Assert.Equal(-280623243, x.Message.MigrateFromChatID.Value);
                    }
                );
        }

        [Fact]
        public void DeleteChatPhotoMessagesCanBeParsed()
        {
            string strGetUpdatesResult = "{\"ok\":true,\"result\":[{\"update_id\":519015214,\n\"message\":{\"message_id\":3,\"from\":{\"id\":195152195,\"is_bot\":false,\"first_name\":\"Pieter-Bas\",\"username\":\"PieterBass\",\"language_code\":\"en-US\"},\"chat\":{\"id\":-1001255131662,\"title\":\"Onno Test Supergroup (With new title)\",\"type\":\"supergroup\"},\"date\":1524904793,\"delete_chat_photo\":true}}]}";
            var objGetUpdatesResult = _rsDeserializer.Deserialize<Result<List<GetUpdatesResult>>>(new RestSharp.RestResponse { Content = strGetUpdatesResult });
            Assert.Collection(objGetUpdatesResult.Data,
                (x) => { Assert.True(x.Message.DeleteChatPhoto); }
                );
        }

        [Fact]
        public void NewChatMembersMessagesCanBeParsed()
        {
            string strGetUpdatesResult = "{\"ok\":true,\"result\":[{\"update_id\":519015215,\n\"message\":{\"message_id\":5,\"from\":{\"id\":195152195,\"is_bot\":false,\"first_name\":\"Pieter-Bas\",\"username\":\"PieterBass\",\"language_code\":\"en-US\"},\"chat\":{\"id\":-1001255131662,\"title\":\"Onno Test Supergroup (With new title)\",\"type\":\"supergroup\"},\"date\":1524905079,\"new_chat_participant\":{\"id\":512941012,\"is_bot\":false,\"first_name\":\"T\",\"last_name\":\"Ester\"},\"new_chat_member\":{\"id\":512941012,\"is_bot\":false,\"first_name\":\"T\",\"last_name\":\"Ester\"},\"new_chat_members\":[{\"id\":512941012,\"is_bot\":false,\"first_name\":\"T\",\"last_name\":\"Ester\"}]}}]}";
            var objGetUpdatesResult = _rsDeserializer.Deserialize<Result<List<GetUpdatesResult>>>(new RestSharp.RestResponse { Content = strGetUpdatesResult });
            Assert.Collection(objGetUpdatesResult.Data,
                (x) =>
                {
                    Assert.Collection(x.Message.NewChatMembers,
                        (y) => { Assert.Equal("T", y.FirstName); Assert.Equal(512941012, y.ID); }
                        );
                }
                );
        }

        [Fact]
        public void StickerMessagesCanBeParsed()
        {
            string strGetUpdatesResult = "{\"ok\":true,\"result\":[{\"update_id\":519015216,\n\"message\":{\"message_id\":7,\"from\":{\"id\":195152195,\"is_bot\":false,\"first_name\":\"Pieter-Bas\",\"username\":\"PieterBass\",\"language_code\":\"en-US\"},\"chat\":{\"id\":-1001255131662,\"title\":\"Onno Test Supergroup (With new title)\",\"type\":\"supergroup\"},\"date\":1524905600,\"sticker\":{\"width\":512,\"height\":512,\"emoji\":\"\\ud83d\\udc81\",\"set_name\":\"JeHoofd\",\"thumb\":{\"file_id\":\"AAQCABML3AMOAARql4cedhBgcv6EAAIC\",\"file_size\":5678,\"width\":128,\"height\":128},\"file_id\":\"CAADAgADuQAD6_gvAWpymsXNz_rFAg\",\"file_size\":35146}}}]}";
            var objGetUpdatesResult = _rsDeserializer.Deserialize<Result<List<GetUpdatesResult>>>(new RestSharp.RestResponse { Content = strGetUpdatesResult });
            Assert.Collection(objGetUpdatesResult.Data,
                (x) =>
                {
                    Assert.Equal("JeHoofd", x.Message.Sticker.SetName);
                    Assert.Equal("AAQCABML3AMOAARql4cedhBgcv6EAAIC", x.Message.Sticker.Thumb.FileID);
                    Assert.Equal("CAADAgADuQAD6_gvAWpymsXNz_rFAg", x.Message.Sticker.FileID);
                    Assert.Equal(35146, x.Message.Sticker.FileSize);
                }
                );
        }

        [Fact]
        public void VideoMessagesCanBeParsed()
        {
            string strGetUpdatesResult = "{\"ok\":true,\"result\":[{\"update_id\":519015217,\n\"message\":{\"message_id\":9,\"from\":{\"id\":195152195,\"is_bot\":false,\"first_name\":\"Pieter-Bas\",\"username\":\"PieterBass\",\"language_code\":\"en-US\"},\"chat\":{\"id\":-1001255131662,\"title\":\"Onno Test Supergroup (With new title)\",\"type\":\"supergroup\"},\"date\":1524905819,\"video\":{\"duration\":30,\"width\":1280,\"height\":720,\"mime_type\":\"video/mp4\",\"thumb\":{\"file_id\":\"AAQEABOfEp0aAARiTsg-iFVbjkZnAAIC\",\"file_size\":1651,\"width\":90,\"height\":50},\"file_id\":\"BAADBAAD9gQAAlC7IFMsuy3IURBEDQI\",\"file_size\":1424455}}}]}";
            var objGetUpdatesResult = _rsDeserializer.Deserialize<Result<List<GetUpdatesResult>>>(new RestSharp.RestResponse { Content = strGetUpdatesResult });
            Assert.Collection(objGetUpdatesResult.Data,
                (x) =>
                {
                    Assert.Equal("video/mp4", x.Message.Video.MimeType);
                    Assert.Equal("AAQEABOfEp0aAARiTsg-iFVbjkZnAAIC", x.Message.Video.Thumb.FileID);
                    Assert.Equal(1651, x.Message.Video.Thumb.FileSize);
                    Assert.Equal(90, x.Message.Video.Thumb.Width);
                    Assert.Equal(50, x.Message.Video.Thumb.Height);
                    Assert.Equal("BAADBAAD9gQAAlC7IFMsuy3IURBEDQI", x.Message.Video.FileID);
                    Assert.Equal(1424455, x.Message.Video.FileSize);
                }
                );
        }

        [Fact]
        public void DocumentMessagesCanBeParsed()
        {
            string strGetUpdatesResult = "{\"ok\":true,\"result\":[{\"update_id\":519015218,\n\"message\":{\"message_id\":11,\"from\":{\"id\":195152195,\"is_bot\":false,\"first_name\":\"Pieter-Bas\",\"username\":\"PieterBass\",\"language_code\":\"en-US\"},\"chat\":{\"id\":-1001255131662,\"title\":\"Onno Test Supergroup (With new title)\",\"type\":\"supergroup\"},\"date\":1524906010,\"document\":{\"file_name\":\"ProSAFE Plus Utility 2.5.3_Release_Notes.html\",\"mime_type\":\"text/html\",\"file_id\":\"BQADBAAD9wQAAlC7IFMu--tmUolTWwI\",\"file_size\":228}}}]}";
            var objGetUpdatesResult = _rsDeserializer.Deserialize<Result<List<GetUpdatesResult>>>(new RestSharp.RestResponse { Content = strGetUpdatesResult });
            Assert.Collection(objGetUpdatesResult.Data,
                (x) =>
                {
                    Assert.Equal("text/html", x.Message.Document.MimeType);
                    Assert.Equal("BQADBAAD9wQAAlC7IFMu--tmUolTWwI", x.Message.Document.FileID);
                    Assert.Equal(228, x.Message.Document.FileSize);
                    Assert.Equal("ProSAFE Plus Utility 2.5.3_Release_Notes.html", x.Message.Document.Filename);
                }
                );
        }

        [Fact]
        public void LocationMessagesCanBeParsed()
        {
            string strGetUpdatesResult = "{\"ok\":true,\"result\":[{\"update_id\":519015219,\n\"message\":{\"message_id\":13,\"from\":{\"id\":195152195,\"is_bot\":false,\"first_name\":\"Pieter-Bas\",\"username\":\"PieterBass\",\"language_code\":\"en-NL\"},\"chat\":{\"id\":-1001255131662,\"title\":\"Onno Test Supergroup (With new title)\",\"type\":\"supergroup\"},\"date\":1524906254,\"location\":{\"latitude\":52.546877,\"longitude\":5.246061}}}]}";
            var objGetUpdatesResult = _rsDeserializer.Deserialize<Result<List<GetUpdatesResult>>>(new RestSharp.RestResponse { Content = strGetUpdatesResult });
            Assert.Collection(objGetUpdatesResult.Data,
                (x) =>
                {
                    Assert.Equal(52.546877, x.Message.Location.Latitude);
                    Assert.Equal(5.246061, x.Message.Location.Longitude);
                }
                );
        }

        [Fact]
        public void AudioMessagesCanBeParsed()
        {
            string strGetUpdatesResult = "{\"ok\":true,\"result\":[{\"update_id\":519015220,\n\"message\":{\"message_id\":15,\"from\":{\"id\":195152195,\"is_bot\":false,\"first_name\":\"Pieter-Bas\",\"username\":\"PieterBass\",\"language_code\":\"en-US\"},\"chat\":{\"id\":-1001255131662,\"title\":\"Onno Test Supergroup (With new title)\",\"type\":\"supergroup\"},\"date\":1524906566,\"audio\":{\"duration\":6,\"mime_type\":\"audio/x-vorbis+ogg\",\"file_id\":\"CQADBAAD_AQAAlC7IFPEhIxDuSTHIQI\",\"file_size\":105243}}}]}";
            var objGetUpdatesResult = _rsDeserializer.Deserialize<Result<List<GetUpdatesResult>>>(new RestSharp.RestResponse { Content = strGetUpdatesResult });
            Assert.Collection(objGetUpdatesResult.Data,
                (x) =>
                {
                    Assert.Equal(6, x.Message.Audio.DurationInSeconds);
                    Assert.Equal("audio/x-vorbis+ogg", x.Message.Audio.MimeType);
                    Assert.Equal("CQADBAAD_AQAAlC7IFPEhIxDuSTHIQI", x.Message.Audio.FileID);
                    Assert.Equal(105243, x.Message.Audio.FileSize);
                }
                );
        }

        [Fact]
        public void VideoNoteMessagesCanBeParsed()
        {
            string strGetUpdatesResult = "{\"ok\":true,\"result\":[{\"update_id\":519015221,\n\"message\":{\"message_id\":17,\"from\":{\"id\":195152195,\"is_bot\":false,\"first_name\":\"Pieter-Bas\",\"username\":\"PieterBass\",\"language_code\":\"en-NL\"},\"chat\":{\"id\":-1001255131662,\"title\":\"Onno Test Supergroup (With new title)\",\"type\":\"supergroup\"},\"date\":1524906705,\"video_note\":{\"duration\":2,\"length\":240,\"thumb\":{\"file_id\":\"AAQEABPLG5waAAQ3SSC3Tggb3Tk7AAIC\",\"file_size\":2001,\"width\":90,\"height\":90},\"file_id\":\"DQADBAAD_QQAAlC7IFPQDkHx_u4ZgQI\",\"file_size\":100804}}}]}";
            var objGetUpdatesResult = _rsDeserializer.Deserialize<Result<List<GetUpdatesResult>>>(new RestSharp.RestResponse { Content = strGetUpdatesResult });
            Assert.Collection(objGetUpdatesResult.Data,
                (x) =>
                {
                    Assert.Equal(2, x.Message.VideoNote.Duration);
                    Assert.Equal(240, x.Message.VideoNote.Length);
                    Assert.Equal("AAQEABPLG5waAAQ3SSC3Tggb3Tk7AAIC", x.Message.VideoNote.Thumb.FileID);
                    Assert.Equal(2001, x.Message.VideoNote.Thumb.FileSize);
                    Assert.Equal(90, x.Message.VideoNote.Thumb.Width);
                    Assert.Equal(90, x.Message.VideoNote.Thumb.Height);
                    Assert.Equal("DQADBAAD_QQAAlC7IFPQDkHx_u4ZgQI", x.Message.VideoNote.FileID);
                    Assert.Equal(100804, x.Message.VideoNote.FileSize);
                }
                );
        }

        [Fact]
        public void LeftChatMemberMessagesCanBeParsed()
        {
            string strGetUpdatesResult = "{\"ok\":true,\"result\":[{\"update_id\":519015222,\n\"message\":{\"message_id\":19,\"from\":{\"id\":195152195,\"is_bot\":false,\"first_name\":\"Pieter-Bas\",\"username\":\"PieterBass\",\"language_code\":\"en-NL\"},\"chat\":{\"id\":-1001255131662,\"title\":\"Onno Test Supergroup (With new title)\",\"type\":\"supergroup\"},\"date\":1524907180,\"left_chat_participant\":{\"id\":512941012,\"is_bot\":false,\"first_name\":\"T\",\"last_name\":\"Ester\"},\"left_chat_member\":{\"id\":512941012,\"is_bot\":false,\"first_name\":\"T\",\"last_name\":\"Ester\"}}}]}";
            var objGetUpdatesResult = _rsDeserializer.Deserialize<Result<List<GetUpdatesResult>>>(new RestSharp.RestResponse { Content = strGetUpdatesResult });
            Assert.Collection(objGetUpdatesResult.Data,
                (x) =>
                {
                    Assert.Equal(512941012, x.Message.LeftChatMember.ID);
                    Assert.Equal("T", x.Message.LeftChatMember.FirstName);
                    Assert.Equal("Ester", x.Message.LeftChatMember.LastName);
                    Assert.False(x.Message.LeftChatMember.IsBot);
                }
                );
        }

        [Fact]
        public void PinedMessageMessagesCanBeParsed()
        {
            string strGetUpdatesResult = "{\"ok\":true,\"result\":[{\"update_id\":519015223,\n\"message\":{\"message_id\":21,\"from\":{\"id\":195152195,\"is_bot\":false,\"first_name\":\"Pieter-Bas\",\"username\":\"PieterBass\",\"language_code\":\"en-US\"},\"chat\":{\"id\":-1001255131662,\"title\":\"Onno Test Supergroup (With new title)\",\"type\":\"supergroup\"},\"date\":1524907368,\"pinned_message\":{\"message_id\":20,\"from\":{\"id\":464986361,\"is_bot\":true,\"first_name\":\"OnnoTestBot\",\"username\":\"OnnoTestBot\"},\"chat\":{\"id\":-1001255131662,\"title\":\"Onno Test Supergroup (With new title)\",\"type\":\"supergroup\"},\"date\":1524907201,\"text\":\"Hello world!\",\"entities\":[{\"offset\":0,\"length\":9,\"type\":\"bold\"}]}}}]}";
            var objGetUpdatesResult = _rsDeserializer.Deserialize<Result<List<GetUpdatesResult>>>(new RestSharp.RestResponse { Content = strGetUpdatesResult });
            Assert.Collection(objGetUpdatesResult.Data,
                (x) =>
                {
                    Assert.Equal(20, x.Message.PinnedMessage.MessageID);
                    Assert.Equal("Hello world!", x.Message.PinnedMessage.Text);
                }
                );
        }

        // TODO: Voice
        // TODO: Venue
    }
}
