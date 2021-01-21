﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Flow.Reactive.Services;
using graph.drawer.Flow.Streams;
using yaml.parser;

namespace graph.drawer.Flow.Nanos
{

    class OrderParserNano : QueryNano<SelectedFile>
    {

        public override IObservable<Unit> Connect()
            => QueryAndCombineQuery<ParserConfiguration>()
              .Select(combine => (file: combine.FirstQuery, config: combine.SecondQuery))
              .Where(payload => payload.file.Path != default)
              .Update<
                       (SelectedFile file, ParserConfiguration config),
                       ParsedResult
               >(this,
                 (payload, result) => {
                     try
                     {
                         var yaml = File.ReadAllText(payload.file.Path.FullName);
                         var parsed = _parser.Parse(yaml, ChartMode.Install);
                         result.Update(parsed);
                     } catch (Exception e) { result.Error(e); }
                 });


        #region construction

        private readonly Parser _parser;

        public OrderParserNano(Parser parser)
        {
            _parser = parser;
        }

        #endregion

    }

}
