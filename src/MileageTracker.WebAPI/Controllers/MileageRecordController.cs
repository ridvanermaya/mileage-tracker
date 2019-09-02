using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MileageTracker.WebAPI.Models;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace MileageTracker.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MileageRecordController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MileageRecordController(ApplicationDbContext context)
        {
            _context = context;
          
            RecurringJob.AddOrUpdate("WeeklySms",
                () => SendWeeklySms(), Cron.Weekly
            );

            RecurringJob.AddOrUpdate("DailySms",
                () => SendDailySms(), Cron.Daily
            );

            // MailMessage mail = new MailMessage();
            //     SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

            //     mail.From = new MailAddress("mileagetrackerMT@gmail.com");
            //     mail.To.Add("ridvanermaya@gmail.com");
            //     mail.Subject = "Test Mail";
            //     mail.Body = "This is for testing SMTP mail from GMAIL";

            //     System.Net.Mail.Attachment attachment;
            //     attachment = new System.Net.Mail.Attachment("your attachment file");
            //     mail.Attachments.Add(attachment);

            //     SmtpServer.Port = 587;
            //     SmtpServer.Credentials = new System.Net.NetworkCredential("mileagetrackerMT@gmail.com", "qgkalukcenddlawk");
            //     SmtpServer.EnableSsl = true;

            //     SmtpServer.Send(mail);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MileageRecord>>> GetMileageRecords()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            userId = "ce693acd-d08d-4303-9e5d-a26b832abc38";
            var mileageRecords = await _context.MileageRecords.Where(x => x.UserId == userId).Include(x => x.User).ToListAsync();

            return mileageRecords;
        }

        public List<MileageRecord> GetMileageRecordsList()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            userId = "ce693acd-d08d-4303-9e5d-a26b832abc38";
            var mileageRecords = _context.MileageRecords.Where(x => x.UserId == userId).Include(x => x.User).ToList();
            return mileageRecords;
        }

        [HttpGet("{MileageRecordId}")]
        public async Task<ActionResult<MileageRecord>> GetMileageRecordById(int mileageRecordId)
        {
            var mileageRecord = await _context.MileageRecords.Include(x => x.User).FirstOrDefaultAsync(x => x.MileageRecordId == mileageRecordId);

            if (mileageRecord == null)
            {
                return NotFound();
            }
            return mileageRecord;
        }

        [HttpPost]
        public async Task<ActionResult<MileageRecord>> AddMileageRecord(MileageRecord mileageRecord)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            userId = "ce693acd-d08d-4303-9e5d-a26b832abc38";

            MileageRecord newMileageRecord = new MileageRecord();
            
            newMileageRecord.Service = mileageRecord.Service.ToUpper();
            newMileageRecord.Mileage = mileageRecord.Mileage;
            newMileageRecord.StartDateTime = mileageRecord.StartDateTime;
            newMileageRecord.EndDateTime = mileageRecord.EndDateTime;
            newMileageRecord.UserId = userId;

            await _context.MileageRecords.AddAsync(newMileageRecord);
            await _context.SaveChangesAsync();
            
            return NoContent();
        }

        [HttpDelete("{MileageRecordId}")]
        public async Task<ActionResult> DeleteMileageRecord(int mileageRecordId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            userId = "ce693acd-d08d-4303-9e5d-a26b832abc38";
            var foundMileageRecord = await _context.MileageRecords.FirstOrDefaultAsync(x => x.MileageRecordId == mileageRecordId && x.UserId == userId);

            if (foundMileageRecord == null)
            {
                return NotFound();
            }

            _context.MileageRecords.Remove(foundMileageRecord);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        public async Task<ActionResult> SendDailySms()
        {
            var userProfiles = await _context.UserProfiles.ToListAsync();

            foreach (var userProfile in userProfiles)
            {
                const string accountSid = "";
                const string authToken = "";
                
                TwilioClient.Init(accountSid, authToken);

                var user = await _context.Users.Where(x => x.Id == userProfile.UserId).FirstOrDefaultAsync();
                var dailyMileageRecords = await _context.MileageRecords.Where(x => x.UserId == user.Id && x.EndDateTime.ToString("MM/dd/yyyy") == DateTime.Today.ToString("MM/dd/yyyy")).ToListAsync();

                var distance = 0.00;

                foreach(var dailyMileageRecord in dailyMileageRecords)
                {
                    distance += dailyMileageRecord.Mileage;
                }

                var messageBody = ($"Hi {userProfile.FirstName}, Mileage Tracker is here! Today you drove {distance} miles and you have {dailyMileageRecords.Count()} record(s). Thanks for using MT!");
                var userPhoneNumber = "+13305031640";

                var message = MessageResource.Create(
                    body: messageBody,
                    from: new Twilio.Types.PhoneNumber("+18142921069"),
                    to: userPhoneNumber
                );
            }

            return NoContent();
        }

        public async Task<ActionResult> SendWeeklySms()
        {
            var userProfiles = await _context.UserProfiles.ToListAsync();

            foreach (var userProfile in userProfiles)
            {
                const string accountSid = "";
                const string authToken = "";
                
                TwilioClient.Init(accountSid, authToken);

                var user = await _context.Users.Where(x => x.Id == userProfile.UserId).FirstOrDefaultAsync();
                var monthlyMileageRecords = await _context.MileageRecords.Where(x => x.UserId == user.Id && x.EndDateTime.Month.ToString("MM/dd/yyyy") == DateTime.Now.Month.ToString("MM/dd/yyyy")).ToListAsync();

                var distance = 0.00;

                foreach(var dailyMileageRecord in monthlyMileageRecords)
                {
                    distance += dailyMileageRecord.Mileage;
                }

                var messageBody = ($"Hi {userProfile.FirstName}, Mileage Tracker is here! This month you drove {distance} miles and you have {monthlyMileageRecords.Count()} record(s). Thanks for using MT!");
                var userPhoneNumber = "+1" + userProfile.PhoneNumber;

                var message = MessageResource.Create(
                    body: messageBody,
                    from: new Twilio.Types.PhoneNumber("+18142921069"),
                    to: userPhoneNumber
                );
            }

            return NoContent();
        }

        [HttpGet("Report")]
        public ActionResult Report()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            MileageRecordReport mileageRecordReport = new MileageRecordReport();
            byte[] abytes = mileageRecordReport.PrepareReport(GetMileageRecordsList());
            
            File(abytes, "applciation/pdf");

            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

            mail.From = new MailAddress("mileagetrackerMT@gmail.com");
            mail.To.Add(userEmail);
            mail.Subject = "Mileage Record Report";
            mail.Body = "The pdf file for mileage records is attached. Thanks for using Mileage Tracker!";

            System.Net.Mail.Attachment attachment;
            attachment = new System.Net.Mail.Attachment("MileageRecord.pdf");
            mail.Attachments.Add(attachment);

            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential("mileagetrackerMT@gmail.com", "qgkalukcenddlawk");
            SmtpServer.EnableSsl = true;

            SmtpServer.Send(mail);

            return NoContent();
        }
    }
}