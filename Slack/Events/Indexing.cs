﻿#region

using System;
using System.Linq;
using Sitecore.Events;
using Slack.Contracts;
using Slack.Models;
using Slack.Services;

#endregion

namespace Slack.Events
{
    // events don't fire for indexing
    public class Indexing
    {
        #region Fields

        private readonly ISlackMessage _message;
        private readonly ISlackService _service;

        #endregion

        #region Constructors

        public Indexing()
        {
            _message = new SlackMessage();
            _service = new SlackService();
        }

        public Indexing(ISlackService service, ISlackMessage message)
        {
            _message = message;
            _service = service;
        }

        #endregion

        #region Methods

        public void OnIndexingStart(object sender, EventArgs args)
        {
            var publications = _service.GetApplicablePublications(new Guid(Constants.EventIds.OnIndexingStart));
            if (!publications.Any())
                return;

            string index = (string)Sitecore.Events.Event.ExtractParameter(args, 0);
            if (string.IsNullOrEmpty(index)) return;

            foreach (var publication in publications)
            {
                foreach (var channel in publication.GetChannels())
                {
                    _message.Text = PopulateIndexingMessage(publication, index, "was started");
                    _message.UpdateChannelInfo(channel, publication);
                    _service.PublishMessage(_message);
                }
            }
        }

        public void OnIndexingEnd(object sender, EventArgs args)
        {
            var publications = _service.GetApplicablePublications(new Guid(Constants.EventIds.OnIndexingEnd));
            if (!publications.Any())
                return;

            string index = (string)Sitecore.Events.Event.ExtractParameter(args, 0);
            if (string.IsNullOrEmpty(index)) return;

            foreach (var publication in publications)
            {
                foreach (var channel in publication.GetChannels())
                {
                    _message.Text = PopulateIndexingMessage(publication, index, "was finished");
                    _message.UpdateChannelInfo(channel, publication);
                    _service.PublishMessage(_message);
                }
            }
        }

        private static string PopulateIndexingMessage(Publication publication, string index, string action)
        {
            var message = string.Empty;
            if (!string.IsNullOrEmpty(publication.Message))
            {
                message = publication.Message + "\n";
            }
            message +=
                $"{index} reindex {action}\n";
            return message;

        }

        #endregion
    }
}