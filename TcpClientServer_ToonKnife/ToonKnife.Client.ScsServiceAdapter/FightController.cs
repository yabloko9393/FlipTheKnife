﻿using ScsService.Client;
using ScsService.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ToonKnife.Common;
using ToonKnife.Common.ScsMessages;


namespace ToonKnife.Client.ScsServiceAdapter
{
    public class FightController
    {
        readonly ScsClient _client;
        public int KnifeIndex;

        public event Action<int> FightEnd;
        public event Action FightStart;
        public event Action<KnifeThrowEventArg> KnifeThrow;

        public FightController(ScsClient client, int knifeIndex)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            KnifeIndex = knifeIndex;
            _client.MsgReaders.RegisterMsgReader<EndFightMessage>(EndFightMessage_Reader);
            _client.MsgReaders.RegisterMsgReader<ThrowKnifeMessage>(ThrowKnifeMessage_Reader);
            _client.MsgReaders.RegisterMsgReader<StartFightMessage>(StartFightMessage_Reader);
        }


        public void SendReady()
        {
            Console.WriteLine("SendReady-------");
            _client.Messenger.SendMessage(new ReadyToFightMessage());
        }

        public void SendThrow(float input)
        {
            Console.WriteLine("Send Input-------");
            ThrowKnifeMessage throwKnife = new ThrowKnifeMessage(input);
            _client.Messenger.SendMessage(throwKnife);
        }

        //---readers

        void StartFightMessage_Reader(ReceivedMsg receivedMsg, StartFightMessage msg)
        {
            if (FightStart != null)
                FightStart.Invoke();
        }

        void ThrowKnifeMessage_Reader(ReceivedMsg receivedMsg, ThrowKnifeMessage msg)
        {
            KnifeThrowEventArg arg = new KnifeThrowEventArg(
                msg.KnifeId,
                msg.Input,
                msg.TimeNextThrow,
                msg.TimeThrow);

            if (KnifeThrow != null)
                KnifeThrow.Invoke(arg);
        }

        void EndFightMessage_Reader(ReceivedMsg receivedMsg, EndFightMessage msg)
        {
            if (FightEnd != null)
                FightEnd.Invoke(msg.WinnerKnifeIndex);
        }
    }
}