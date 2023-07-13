using Avito2.Abstract;
using Avito2.Data.Repositories;
using Avito2.Domains;

namespace Avito2.BlazorServices
{
    public class MessageService
    {
        private readonly IRepository<Message> _messagesRepository;
        private readonly IRepository<Advertisement> _advertisementRepository;

        public MessageService(IRepository<Message> messagesRepository, IRepository<Advertisement> advertisementRepository)
        {
            _messagesRepository = messagesRepository;
            _advertisementRepository = advertisementRepository;
        }

        public Advertisement GetAdvertisement(long id)
        {
            return _advertisementRepository.Read(id);
        }

        public IEnumerable<Message> GetMessages(string currentUser, string interlocutor, long advertisement)
        {
            var ad = _advertisementRepository.Read(advertisement);

            return _messagesRepository
                .ReadList()
                .Where(x => (x.ReceiverId == interlocutor && x.SenderId == currentUser && x.Advertisement == ad)
                            || (x.ReceiverId == currentUser && x.SenderId == interlocutor && x.Advertisement == ad));
        }

        public long? AddMessage(DateTime date, string? interlocutorId, string currentUserId, string? messageText, long adId)
        {
            try
            {
                var ad = _advertisementRepository.ReadList().FirstOrDefault(x => x.Id == adId);

                var m = new Message();
                m.Date = DateTime.Now;
                m.ReceiverId = interlocutorId;
                m.SenderId = currentUserId;
                m.Text = messageText;
                m.Advertisement = ad;

                _messagesRepository.Create(m);

                return m.Id;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
