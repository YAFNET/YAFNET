/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
 * https://www.yetanotherforum.net/
 *
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Tests.TestInfrastructure;

using System.Threading.Tasks;

using YAF.Types.Interfaces.Services;
using YAF.Types.Objects;

/// <summary>
/// An in-memory <see cref="ISearch"/> that records index changes instead of writing a Lucene index.
/// </summary>
public class FakeSearch : ISearch
{
    /// <summary>
    /// Gets the indexed messages by message ID.
    /// </summary>
    public Dictionary<int, SearchMessage> Indexed { get; } = new();

    /// <summary>
    /// Gets the message IDs removed with <see cref="DeleteSearchIndexRecordByMessageId"/>.
    /// </summary>
    public List<int> DeletedMessageIds { get; } = [];

    /// <summary>
    /// Gets the topic IDs removed with <see cref="DeleteSearchIndexRecordByTopicId"/>.
    /// </summary>
    public List<int> DeletedTopicIds { get; } = [];

    public void Optimize()
    {
    }

    public bool ClearSearchIndex()
    {
        this.Indexed.Clear();
        return true;
    }

    public void DeleteSearchIndexRecordByMessageId(int messageId)
    {
        this.DeletedMessageIds.Add(messageId);
        this.Indexed.Remove(messageId);
    }

    public void DeleteSearchIndexRecordByTopicId(int topicId)
    {
        this.DeletedTopicIds.Add(topicId);

        foreach (var id in this.Indexed.Where(i => i.Value.TopicId == topicId).Select(i => i.Key).ToList())
        {
            this.Indexed.Remove(id);
        }
    }

    public Task AddSearchIndexAsync(IEnumerable<SearchMessage> messageList)
    {
        messageList.ForEach(this.AddSearchIndexItem);
        return Task.CompletedTask;
    }

    public void AddSearchIndexItem(SearchMessage message)
    {
        this.Indexed[message.MessageId ?? 0] = message;
    }

    public void UpdateSearchIndexItem(SearchMessage message, bool dispose = false)
    {
        this.AddSearchIndexItem(message);
    }

    public int CountHits(string input)
    {
        return this.Indexed.Values.Count(m => m.Message?.IndexOf(input, StringComparison.OrdinalIgnoreCase) >= 0);
    }

    public List<SearchMessage> SearchSimilar(string filter, string input, string fieldName = "")
    {
        return [];
    }

    public List<SearchMessage> SearchPaged(
        out int totalHits,
        int forumId,
        string input,
        int pageIndex,
        int pageSize,
        string fieldName = "")
    {
        totalHits = 0;
        return [];
    }

    public List<SearchMessage> SearchDefault(int forumId, string input, string fieldName = "")
    {
        return [];
    }
}
