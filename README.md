# rocketAiProject


## Required Content
Required Content:
General discussion around project purpose
showing the rockets landing
show all the agents landing together
suggestion: include overhead shot of the platforms blinking green/red
show the UI interaction and settings
showing camera controls and switching (each num changes to a different camera)
stats text; two purposes
first, help diagnose model issues during training
second, shows observers the performance of a model during inference
Code Showcase
scan through the AI training script
scan through the model yaml config
orbital camera code

## Timeline
Potential Timeline Draft:
General discussion around project purpose
Motivations:
Increasing accessibility and content for space to increase interest in the topic
Exploration of AI into space travel applications
Product: program showcasing an AI trained to land a rocket
visual: likely clip art
Time estimate: ~ 45 seconds
Transition:
showing the rockets landing
show all the agents landing together
first show the main subject, pan the camera down the show the platform
show up to the point of collision to show the color change
fade to overhead shot of the platforms blinking green/red
during this part, commentate on what is happening
key points:
use of multiple agents to explore more possibilities and speed up training
color changing platforms to give visual guide on performance
Time estimate: ~ 45 seconds
fade from the overhead shot back to the main view to show the UI interaction and settings
showing camera controls and switching (each num changes to a different camera)
stats text; two purposes (visual note: probably draw a box or highlight around the text)
first, help diagnose model issues during training
second, shows observers the performance of a model during inference
Time estimate: ~ 40 seconds
Code Showcase
scan through the AI training script (rocket.cs)
Talk about the ML Agent PPO (proximal policy optimization) cycle functions
OnEpisodeBegin
Sets up the initial state of every episode for an agent (refresh)
CollectObservation
Determining what parts of the world the AI is capable of sensing
OnActionReceived
the AI making a decision and us acting out that decision
Evaluation functions:
FixedUpdate
Continuous checks for rewards and punishments to dish out prior to touching the platform (if they do at all)
OnCollisionEnter
Reward or punish collision with the platform based on if the rocket would be damaged post-impact
Success
Calculate how much to reward a success
Fail
Calculate how much to punish a failure
Optional: Heuristic
define manual controls to help you properly control and record for the AI to reference
Time estimate: ~ 50 seconds to a minute
Conclusion Segment
Impact: tie into the intro's point about both increasing interest in space and also exploring frontier technologies like proper rocket landing