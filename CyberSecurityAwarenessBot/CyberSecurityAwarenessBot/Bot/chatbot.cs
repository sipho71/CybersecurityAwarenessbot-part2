
using System;
using System.Collections.Generic;
using System.Linq;

namespace CyberSecurityAwarenessBot
{
    // Delegate used to process a user's message.
    public delegate string ResponseProcessor(string input);

    public class Chatbot
    {
        public string UserName { get; set; }

        // Memory: information the bot remembers during the conversation.
        private string favouriteTopic = "";
        private string currentTopic = "";

        private readonly Random random = new Random();

        // Several responses for each topic (dictionary of lists).
        private readonly Dictionary<string, List<string>> responses =
            new Dictionary<string, List<string>>()
        {
            {
                "password", new List<string>
                {
                    "Use a strong, unique password for every account. A password manager can help.",
                    "Avoid using your birthday or name in passwords. Longer passwords are generally safer.",
                    "Never share your passwords with other people, and enable two-factor authentication."
                }
            },
            {
                "scam", new List<string>
                {
                    "Be careful with messages that pressure you to act quickly or send money.",
                    "Verify suspicious requests using the organisation's official contact details.",
                    "Never share banking details or one-time PINs with someone who contacts you unexpectedly."
                }
            },
            {
                "privacy", new List<string>
                {
                    "Review the privacy settings on your social media accounts.",
                    "Share personal information online only when necessary.",
                    "Check app permissions and remove access that an app does not need."
                }
            },
            {
                "phishing", new List<string>
                {
                    "Check the sender's address before trusting an email.",
                    "Avoid clicking unexpected links. Visit the official website directly instead.",
                    "Be suspicious of messages asking for passwords or personal information."
                }
            },
            {
                "malware", new List<string>
                {
                    "Keep your operating system and security software updated.",
                    "Download applications only from trusted sources.",
                    "Avoid opening unexpected attachments from unknown senders."
                }
            },
            {
                "browsing", new List<string>
                {
                    "Check website addresses carefully before entering personal information.",
                    "Use updated browsers and avoid suspicious downloads.",
                    "HTTPS helps protect information in transit, but it does not guarantee that a website is trustworthy."
                }
            }
        };

        // The GUI calls this delegate to get a chatbot response.
        public ResponseProcessor ProcessResponse { get; }

        public Chatbot(string userName)
        {
            UserName = string.IsNullOrWhiteSpace(userName)
                ? "User"
                : userName.Trim();

            ProcessResponse = GenerateResponse;
        }

        private string GenerateResponse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return "Please enter a message so I can help you.";

            string question = input.Trim().ToLowerInvariant();

            // Memory: remember the user's name.
            if (question.StartsWith("my name is "))
            {
                string name = input.Trim().Substring(11).Trim();

                if (!string.IsNullOrWhiteSpace(name))
                {
                    UserName = name;
                    return $"Nice to meet you, {UserName}! I'll use your name during this conversation.";
                }
            }

            // Memory: remember the user's favourite cybersecurity topic.
            if (question.Contains("i'm interested in ") ||
                question.Contains("i am interested in ") ||
                question.Contains("my favourite topic is ") ||
                question.Contains("my favorite topic is "))
            {
                string topic = FindTopic(question);

                if (!string.IsNullOrEmpty(topic))
                {
                    favouriteTopic = topic;
                    currentTopic = topic;

                    return $"Great, {UserName}! I'll remember that you're interested in {topic}. " +
                           $"It's a crucial part of staying safe online. {GetRandomTip(topic, false)}";
                }
            }

            // Sentiment: worried -> reassure and give a tip straight away.
            if (ContainsAny(question, "worried", "scared", "afraid", "anxious", "nervous"))
            {
                string topic = ResolveTopic(question, "scam");

                return $"It's completely understandable to feel that way, {UserName}. " +
                       "Let me share some tips to help you stay safe. " + GetRandomTip(topic);
            }

            // Sentiment: frustrated / confused / overwhelmed -> encourage and give a tip.
            if (ContainsAny(question, "frustrated", "angry", "overwhelmed",
                "confused", "unsure", "don't understand", "do not understand"))
            {
                string topic = ResolveTopic(question, "password");

                return $"I understand that this can feel frustrating, {UserName}. " +
                       "You don't need to learn everything at once. " + GetRandomTip(topic);
            }

            // Sentiment: curious.
            if (ContainsAny(question, "curious", "interested", "wondering"))
            {
                string topic = ResolveTopic(question, "privacy");

                return $"That's a good question, {UserName}! " + GetRandomTip(topic);
            }

            // Conversation flow: follow-up questions continue the current topic.
            if (ContainsAny(question, "tell me more", "explain more", "another tip",
                "more tips", "explain further", "more details", "give me another"))
            {
                string topic = currentTopic != "" ? currentTopic :
                    favouriteTopic != "" ? favouriteTopic : "password";

                currentTopic = topic;
                return GetRandomTip(topic);
            }

            // Memory recall questions.
            if (question.Contains("what is my name") ||
                question.Contains("do you remember my name"))
            {
                return $"Your name is {UserName}.";
            }

            if (question.Contains("what is my favourite topic") ||
                question.Contains("what is my favorite topic") ||
                question.Contains("what topic am i interested in"))
            {
                return favouriteTopic == ""
                    ? "You haven't told me your favourite cybersecurity topic yet."
                    : $"You told me you're interested in {favouriteTopic}.";
            }

            if (question.Contains("favourite topic") ||
                question.Contains("favorite topic"))
            {
                return "You can tell me, for example, 'I'm interested in privacy', " +
                       "and I'll remember your favourite topic during this conversation.";
            }

            // General inquiries.
            if (question.Contains("how are you"))
                return $"I'm doing well, {UserName}! I'm ready to help you stay safe online.";

            if (question.Contains("purpose") ||
                question.Contains("what do you do"))
            {
                return "My purpose is to help you understand cybersecurity and recognise " +
                       "common online threats.";
            }

            if (question.Contains("what can i ask"))
            {
                return "You can ask me about passwords, scams, privacy, phishing, " +
                       "malware and safe browsing.";
            }

            // Keyword recognition.
            string matchedTopic = FindTopic(question);

            if (matchedTopic != "")
            {
                currentTopic = matchedTopic;
                return GetRandomTip(matchedTopic);
            }

            // Greetings.
            if (question == "hi" || question == "hello" || question == "hey" ||
                question.StartsWith("hi ") || question.StartsWith("hello ") ||
                question.StartsWith("hey "))
            {
                return $"Hello, {UserName}! What would you like to learn about today?";
            }

            // Default response for unknown input.
            return "I'm not sure I understand. Can you try rephrasing? " +
                   "You can ask about passwords, scams, privacy, phishing, malware or safe browsing.";
        }

        // Finds a topic in the message, otherwise falls back to the current topic,
        // then to the supplied default. Also updates the current topic.
        private string ResolveTopic(string question, string defaultTopic)
        {
            string topic = FindTopic(question);

            if (topic == "")
                topic = currentTopic != "" ? currentTopic : defaultTopic;

            currentTopic = topic;
            return topic;
        }

        private string FindTopic(string question)
        {
            if (question.Contains("password"))
                return "password";

            if (question.Contains("scam") || question.Contains("fraud"))
                return "scam";

            if (question.Contains("privacy") || question.Contains("personal information"))
                return "privacy";

            if (question.Contains("phishing") || question.Contains("suspicious email"))
                return "phishing";

            if (question.Contains("malware") || question.Contains("virus"))
                return "malware";

            if (question.Contains("safe browsing") ||
                question.Contains("browsing") ||
                question.Contains("website"))
                return "browsing";

            return "";
        }

        // Picks a random tip. When the topic matches the user's remembered
        // favourite topic, the tip is personalised.
        private string GetRandomTip(string topic, bool personalise = true)
        {
            if (!responses.ContainsKey(topic))
                topic = "password";

            List<string> tips = responses[topic];
            string tip = tips[random.Next(tips.Count)];

            return personalise ? Personalise(topic, tip) : tip;
        }

        private string Personalise(string topic, string tip)
        {
            if (favouriteTopic != "" && topic == favouriteTopic)
                return $"As someone interested in {favouriteTopic}, {UserName}, you might like this: {tip}";

            return tip;
        }

        private bool ContainsAny(string input, params string[] keywords)
        {
            return keywords.Any(keyword => input.Contains(keyword));
        }
    }
}
