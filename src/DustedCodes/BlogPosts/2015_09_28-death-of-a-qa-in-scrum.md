<!--
    Tags: scrum agile testing
    Type: HTML
-->

# Death of a QA in Scrum

<p>The world is changing fast and the software industry even faster. Today I am a software developer with almost 9 years of commercial experience and recently I was told that this is about the time when a developer actually becomes good. Whether that is true or not I leave to someone else, but there is certainly a level of maturity which I have today and I didn't have a couple of years ago.</p>

<p>In those 9 years I was lucky enough to work in different teams, with different technologies and different approaches of agile methodologies.</p>

<p>However, one thing which has never changed was my role within the teams. Regardless of my actual job title I was a hands-on developer among other roles such as testers, business analysts, product owners, architects and UX experts.</p>

<p>The interesting part is that software developers and testers were always separated into two different roles. One guy (or girl!) was supposed to write code and another guy/girl was supposed to test it.</p>

<p>This model sounds great in theory, but in my own (subjective) experience it never really worked.</p>

<h2>The separation of roles in Scrum</h2>

<p>This is not another debate on manual vs. automated testing. When I say it never really worked then I mean in the context of working in an agile Scrum team with automated regression QA.</p>

<p>More precisely the distinction of developers and QA caused a lot of friction in our Scrum process and mostly we ended up with many issues like:</p>

<ul>
    <li>QA had very little work at the beginning of a sprint</li>
    <li>At the end of a sprint we had a lot of QA tasks piling up</li>
    <li>Many QA tasks didn't get done before the end of the sprint</li>
    <li>Developers wrote features quicker than QA could test them</li>
    <li>...and most importantly, it was impossible to have a developer write code for an entire iteration and on the last day have everything go through code review and QA without any issues</li>
</ul>

<p>Basically through the separation of roles we ended up with a lot of difficulties, bottlenecks and inefficient use of resources.</p>

<h3>A production line in Scrum</h3>

<p>What was happening is that a user story got divided into several work tasks and each task was worked on by a different person in the team. It felt a lot like a production line:</p>

<img src="https://storage.googleapis.com/dusted-codes/images/blog-posts/2015-09-28/21794643692_d42d0f0d78_o.png" alt="Scrum User Story Production Line, Image by Dustin Moris Gorski">

<p>While it was possible to do some parallel work on the development and QA task at the same time, it was not possible to close one before the other. We had inter team dependencies.</p>

<p>Unfortunately this production line is not a hybrid approach of Scrum with a pinch of Kanban, it is more of a mini waterfall within Scrum:</p>

<ul>
    <li>The team commits to deliver a fixed scope (sprint backlog) at a fixed date (end of sprint) with fixed resources (size of the team)</li>
    <li>But the problem is that the scope is a set of unrelated work items instead of one or two <strong>shippable features</strong></li>
    <li>Because each work item is divided into different phases (development, review, QA, etc.) it is difficult to estimate it as a whole and the team ends up over-committing</li>
    <li>Dependencies between the phases and bottlenecks only add up to it</li>
    <li>And finally the separation of roles leads to a lot of last minute testing, not allowing any room for re-iteration and unexpected issues</li>
    <li>Eventually the sprint goal will be missed</li>
    <li>And early warning signs like a sprint burndown are not meaningful as the team never really knows if they are on track</li>
</ul>

<p>When this happens the team usually doesn't think of completing one goal anymore. Individual people think of completing individual phases. The separation of roles is the same as the separation of concerns and results in sub teams within a team. While this is great as a coding practise it is poison to Scrum.</p>

<h3>Scrum is one team, not a set of roles</h3>

<p>The problem also becomes very prominent if the team starts using language like "<strong>we</strong> developers do this..., <strong>they</strong> QA do that..." and vice versa.</p>

<p>Scrum is built on the fundamental believe that one self-contained team works together towards a goal. There cannot be any sub teams, dependencies or bottle necks within a team. Each member of the team (except the PO and BA if you like) must be capable of working on each phase of a user story, otherwise your Scrum is deemed to fail.</p>

<p>What does this mean for developers and QA then? Well, there is simply <em>not much space for a dedicated QA</em> anymore!</p>

<p>Yes that's right, but before everyone starts to freak out now, I don't mean to get rid of all QA by sacking them. That would be mad and a true waste of many years of experience, valuable domain knowledge, skills and employee loyalty. We need to get rid of the QA as a separate role within a Scrum team!</p>

<h2>We are all developers</h2>

<p>I think it would be fair to say that we are all developers. At the end of the day writing good automation tests requires a lot of engineering skills and good coding practices as much as writing production code.</p>

<p>In order to make every team member responsible for writing production code as well as reviewing and writing tests we need to give everyone a title which reflects these responsibilities.
</p>

<h3>Make everyone a developer!</h3>

<p>If you really need to distinguish between different levels of expertise then give your team members a title which relates to their experience rather than a role. You can turn your automation QA into a junior or mid-level developer. They already know how to write maintainable automation tests so they are not far away from writing maintainable production code. With a little bit of guidance from a senior member of the team it should be possible to train your current QA into a full member of the team.</p>

<p>Personally I have not made the experience of this change yet, but I witnessed a transition from manual testers to fully committed automation testers and the results were extremely good! The learning curve is very steep in the beginning, but to be honest the entire software industry is a big journey of change and it is not any different for a junior developer either.</p>

<p><strong>As someone said to me before... it takes about 8-9 years before a developer actually becomes good...</strong></p>

<p>Are you ready for the change?</p>
