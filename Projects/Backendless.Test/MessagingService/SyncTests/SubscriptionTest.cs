﻿using System;
using System.Collections.Generic;
using System.Text;
using BackendlessAPI.Async;
using BackendlessAPI.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BackendlessAPI.Test.MessagingService.SyncTests
{
  [TestClass]
  public class SubscriptionTest : TestsFrame
  {
    [TestMethod]
    public void TestBasicMessageSubscription()
    {
      SetLatch();
      SetMessage();

      channel = Backendless.Messaging.Subscribe( TEST_CHANNEL );
      
        channel.AddMessageListener<Message>((message) =>
          {
            try
            {
                if(
                  message.MessageId.Equals(
                    messageStatus.MessageId ) )
                {
                  Assert.AreEqual( message, message.Data,
                                   "Server returned a message with a wrong message data" );
                  Assert.AreEqual( messageStatus.MessageId,
                    message.MessageId,
                                   "Server returned a message with a wrong messageId" );

                  latch.Signal();
                }
            }
            catch( System.Exception e )
            {
              FailCountDownWith( e );
            }            
          });

      messageStatus = Backendless.Messaging.Publish( message, TEST_CHANNEL );

      Assert.IsNotNull( messageStatus.MessageId, "Server didn't set a messageId for the message" );
      Assert.IsTrue( messageStatus.Status.Equals( PublishStatusEnum.PUBLISHED ),
                     "Message status was not set as published" );

      Await();
      CheckResult();
    }

    [TestMethod]
    public void TestBasicMessageSubscriptionWithSubtopic()
    {
      SetLatch();
      SetMessage();

      string subtopic = "sub" + Random.Next();

      SubscriptionOptions subscriptionOptions = new SubscriptionOptions();
      subscriptionOptions.Subtopic = subtopic;
      channel = Backendless.Messaging.Subscribe( TEST_CHANNEL );
       
      channel.AddMessageListener<Message>((message) =>
      {
        try
        {
          if (
            message.MessageId.Equals(
              messageStatus.MessageId))
          {
            Assert.AreEqual(message, message.Data,
              "Server returned a message with a wrong message data");
            Assert.AreEqual(messageStatus.MessageId,
              message.MessageId,
              "Server returned a message with a wrong messageId");

            latch.Signal();
          }
        }
        catch (System.Exception t)
        {
          FailCountDownWith(t);
        }
      });  

      var publishOptions = new PublishOptions();
      publishOptions.Subtopic = subtopic;

      messageStatus = Backendless.Messaging.Publish( message, TEST_CHANNEL, publishOptions );

      Assert.IsNotNull( messageStatus.MessageId, "Server didn't set a messageId for the message" );
      Assert.IsTrue( messageStatus.Status.Equals( PublishStatusEnum.PUBLISHED ),
                     "Message status was not set as published" );

      Await();
      CheckResult();
    }

    [TestMethod]
    public void TestBasicMessageSubscriptionWithSelector()
    {
      SetLatch();
      SetMessage();

      string headerKey = "header_" + Random.Next();
      string headerValue = "someValue";
      headers = new Dictionary<string, string>();
      headers.Add( headerKey, headerValue );

      var subscriptionOptions = new SubscriptionOptions();
      subscriptionOptions.Selector = headerKey + "='" + headerValue + "'";

      channel = Backendless.Messaging.Subscribe( TEST_CHANNEL );
      channel.AddMessageListener<Message>((resultMessage) =>
      {
        try
        {
          if (
            resultMessage.MessageId.Equals(
              messageStatus.MessageId))
          {
            Assert.AreEqual(message, resultMessage.Data,
              "Server returned a message with a wrong message data");

            Assert.IsTrue(
              resultMessage.Headers.ContainsKey(headerKey),
              "Server returned a message with wrong headers");

            Assert.AreEqual(headerValue,
              resultMessage.Headers[headerKey],
              "Server returned a message with wrong headers");

            Assert.AreEqual(messageStatus.MessageId,
              resultMessage.MessageId,
              "Server returned a message with a wrong messageId");

            latch.Signal();
          }
        }
        catch (System.Exception t)
        {
          FailCountDownWith(t);
        }
      });  

      var publishOptions = new PublishOptions {Headers = headers};

      messageStatus = Backendless.Messaging.Publish( message, TEST_CHANNEL, publishOptions );

      Assert.IsNotNull( messageStatus.MessageId, "Server didn't set a messageId for the message" );
      Assert.IsTrue( messageStatus.Status.Equals( PublishStatusEnum.PUBLISHED ),
                     "Message status was not set as published" );

      Await();
      CheckResult();
    }

    [TestMethod]
    public void TestMessageSubscriptionForUnknownChannel()
    {
      try
      {
        Backendless.Messaging.Subscribe( GetRandomstringMessage() );
      }
      catch( System.Exception e )
      {
        CheckErrorCode( 5010, e );
      }
    }
  }
}