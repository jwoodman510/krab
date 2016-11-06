using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Krab.DataAccess.Exception;
using Krab.DataAccess.KeywordResponseSetSubredditReport;
using Report = Krab.DataAccess.KeywordResponseSetSubredditReport.KeywordResponseSetSubredditReport;

namespace Krab.DataAccess.Dac
{
    public interface IKeywordResponseSetSubredditReportDac
    {
        IQueryable<Report> GetByKeywordResponseSets(IEnumerable<int> keywordResponseSetIds);
        
        IQueryable<Report> GetByKeywordResponseSetsDateRange(IEnumerable<int> keywordResponseSetIds, DateTime startDateUtc, DateTime endDateUtc);

        Report Get(int keywordResponseSetId, long subredditId, DateTime dateTimeUtc);

        IQueryable<Report> GetReadyToClose();

        Report IncrementResponseOrCreate(int keywordResponseSetId, long subredditId, DateTime dateTimeUtc, int incrementBy);

        void Close(int keywordResponseSetId, long subredditId, DateTime dateTimeUtc);

        void Delete(int keywordResponseSetId, long subredditId, DateTime dateTimeUtc);
    }

    public class KeywordResponseSetSubredditReportDac : IKeywordResponseSetSubredditReportDac
    {
        private readonly KeywordResponseSetSubredditReportDb _context;

        public KeywordResponseSetSubredditReportDac(KeywordResponseSetSubredditReportDb context)
        {
            _context = context;
        }

        public IQueryable<Report> GetByKeywordResponseSets(IEnumerable<int> keywordResponseSetIds)
        {
            if (keywordResponseSetIds == null)
                return new List<Report>().AsQueryable();

            return _context.KeywordResponseSetSubredditReports
                .AsNoTracking()
                .Where(r => keywordResponseSetIds.Contains(r.KeywordResponseSetId));
        }

        public IQueryable<Report> GetByKeywordResponseSetsDateRange(IEnumerable<int> keywordResponseSetIds, DateTime startDateUtc,
            DateTime endDateUtc)
        {
            if (keywordResponseSetIds == null)
                return new List<Report>().AsQueryable();

            startDateUtc = startDateUtc.Date;
            endDateUtc = endDateUtc.Date;

            return _context.KeywordResponseSetSubredditReports
                .AsNoTracking()
                .Where(r =>
                    keywordResponseSetIds.Contains(r.KeywordResponseSetId) &&
                    r.ReportDateUtc >= startDateUtc &&
                    r.ReportDateUtc <= endDateUtc);
        }

        public Report Get(int keywordResponseSetId, long subredditId, DateTime dateTimeUtc)
        {
            return _context.KeywordResponseSetSubredditReports
                .AsNoTracking()
                .FirstOrDefault(r => r.KeywordResponseSetId == keywordResponseSetId &&
                                     r.SubredditId == subredditId &&
                                     r.ReportDateUtc == dateTimeUtc.Date);
        }

        public IQueryable<Report> GetReadyToClose()
        {
            var delteDaysOlderThan = DateTime.UtcNow.Date.AddDays(-1);

            return _context.KeywordResponseSetSubredditReports
                .AsNoTracking()
                .Where(r => r.ReportDateUtc <= delteDaysOlderThan);
        }

        public Report IncrementResponseOrCreate(int keywordResponseSetId, long subredditId, DateTime dateTimeUtc, int incrementBy)
        {
            if (incrementBy < 1)
                throw new ValidationException("Invalid Increment.");

            var rpt = Find(keywordResponseSetId, subredditId, dateTimeUtc);

            if (rpt == null)
            {
                var created = new Report
                {
                    KeywordResponseSetId = keywordResponseSetId,
                    SubredditId = subredditId,
                    ReportDateUtc = dateTimeUtc.Date,
                    IsClosed = false,
                    NumResponses = incrementBy
                };

                _context.KeywordResponseSetSubredditReports.Add(created);
                _context.SaveChanges();

                return created;
            }

            var entry = _context.Entry(rpt);

            entry.Entity.NumResponses += incrementBy;

            _context.SaveChanges();

            _context.Entry(rpt).State = EntityState.Detached;

            return rpt;
        }

        public void Close(int keywordResponseSetId, long subredditId, DateTime dateTimeUtc)
        {
            var rpt = Find(keywordResponseSetId, subredditId, dateTimeUtc);

            if (rpt == null)
                throw new NotFoundException($"Report not found: {keywordResponseSetId}:{subredditId}:{dateTimeUtc.Date}");

            var entry = _context.Entry(rpt);

            entry.Entity.IsClosed = true;

            _context.SaveChanges();

            _context.Entry(rpt).State = EntityState.Detached;
        }

        public void Delete(int keywordResponseSetId, long subredditId, DateTime dateTimeUtc)
        {
            var existing = Find(keywordResponseSetId, subredditId, dateTimeUtc);

            if (existing == null)
                return;

            var entry = _context.Entry(existing);
            entry.State = EntityState.Deleted;

            _context.SaveChanges();
        }

        private Report Find(int keywordResponseSetId, long subredditId, DateTime dateTimeUtc)
        {
            return _context.KeywordResponseSetSubredditReports
                .Find(keywordResponseSetId, subredditId, dateTimeUtc.Date);
        }
    }
}
