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
            
        Report Get(int keywordResponseSetId, int subredditId, DateTime dateTimeUtc);

        Report IncrementResponseOrCreate(int keywordResponseSetId, int subredditId, DateTime dateTimeUtc);

        void Close(int keywordResponseSetId, int subredditId, DateTime dateTimeUtc);

        void Delete(int keywordResponseSetId, int subredditId, DateTime dateTimeUtc);
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

        public Report Get(int keywordResponseSetId, int subredditId, DateTime dateTimeUtc)
        {
            return _context.KeywordResponseSetSubredditReports
                .AsNoTracking()
                .FirstOrDefault(r => r.KeywordResponseSetId == keywordResponseSetId &&
                                     r.SubredditId == subredditId &&
                                     r.ReportDateUtc == dateTimeUtc.Date);
        }

        public Report IncrementResponseOrCreate(int keywordResponseSetId, int subredditId, DateTime dateTimeUtc)
        {
            var rpt = Find(keywordResponseSetId, subredditId, dateTimeUtc);

            if (rpt == null)
            {
                var created = new Report
                {
                    KeywordResponseSetId = keywordResponseSetId,
                    SubredditId = subredditId,
                    ReportDateUtc = dateTimeUtc.Date,
                    IsClosed = false,
                    NumResponses = 1
                };

                _context.KeywordResponseSetSubredditReports.Add(created);
                _context.SaveChanges();

                return created;
            }

            var entry = _context.Entry(rpt);

            entry.Entity.NumResponses++;

            _context.SaveChanges();

            _context.Entry(rpt).State = EntityState.Detached;

            return rpt;
        }

        public void Close(int keywordResponseSetId, int subredditId, DateTime dateTimeUtc)
        {
            var rpt = Find(keywordResponseSetId, subredditId, dateTimeUtc);

            if (rpt == null)
                throw new NotFoundException($"Report not found: {keywordResponseSetId}:{subredditId}:{dateTimeUtc.Date}");

            var entry = _context.Entry(rpt);

            entry.Entity.IsClosed = true;

            _context.SaveChanges();

            _context.Entry(rpt).State = EntityState.Detached;
        }

        public void Delete(int keywordResponseSetId, int subredditId, DateTime dateTimeUtc)
        {
            var existing = Find(keywordResponseSetId, subredditId, dateTimeUtc);

            if (existing == null)
                return;

            var entry = _context.Entry(existing);
            entry.State = EntityState.Deleted;

            _context.SaveChanges();
        }

        private Report Find(int keywordResponseSetId, int subredditId, DateTime dateTimeUtc)
        {
            return _context.KeywordResponseSetSubredditReports
                .Find(keywordResponseSetId, subredditId, dateTimeUtc.Date);
        }
    }
}
