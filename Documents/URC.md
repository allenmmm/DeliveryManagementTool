# **URC Workshop**

## Introduction
Refer to the [tikiwiki](http://plymweb.goodrich.root.local/tikiwiki/sw_scrum_sprintzerotwo_urcworkshop) for process detail about this event.

#####Details
|Project|Venue|Date|
|---|---|---|
|Delivery Management Tool|Sperry Room|05/08/2019 - 05/08/2019|

#####Attendees
|Name|Role|
|---|---|
|Nathan Roberts|Scrum Master|
|Matt Allen|Development Team|
|Dan Huxham|Development Team|
|Megan Finch|Development Team|
|Katie Snell|Development Team|
|Kerrie Pyburn|Delivery Assurance|
|Kathy Clark|Buyer|
|Gemma Graber|Buyer|
|Dawn Batty|Buyer|
|Kim Legg|Buyer (Indirect?)|
|Bethany Tout|Buyer (Indirect?)|

## Notes
#### User Roles
These are the user roles as described during the workshop:
* Delivery Assurance
    * Represented by Kerrie Pyburn.
    * Ensures the responses have been received and distributing information accordingly; all other users feed off information distributed by DA.
    * Files this information away.
    * Modifies the columns on the Order Book for a few different suppliers, Axiom and STI.
    * DA's involvement with the Order Books may be vastly reduced if the requirements proposed during this URC are realised.
* Buyer
    * Represented by Kathy Clark, Gemma Graber, Dawn Batty, Kim Legg and Bethany Tout.
    * If DA is absent, a buyer (usually Kathy) takes over this role.
* Purchasing Manager/Supply Development
    * Purchasing Managers represented by Nikki Horrell and Richard Jordan.
    * There are various other users who may access the system only to get information. They have no unique requirements and so are grouped under a single user role.
    * Planners are not included in this group as they do not look at the order books. The users that attended the metting weren't interested in giving the planners all of that information either.
* Supply Chain Lead
    * Represented by Toni Revell Requires summary overview of late delivery dates

#### Use Stories
*See "Outputs" section below

#### Business Rules
* Each supplier is managed by a buyer. One supplier only has two different buyers; this is due to the workload related to that supplier being too much for one buyer to handle.
* There are several contacts or email addresses for each supplier, in case one of the contacts is out of office.
* The order book is sent out once per week on a Thursday (possibly during the night before). The 'soft' deadline to receive responses from the supplier is midday of Monday the following week. These timescales are fixed and there is no reason to change them as far as the buyers are concerned.
    * The buyers may want to send out a specific order book to a supplier on a one-off, ad hoc basis, but they would never want to regularly schedule an order book outside the regular Thursday-Monday routine, so a separate input for this when registering a supplier is not neccessary.
* Suppliers must give a reason for change of delivery date. If they fail to do this under the current system, the buyer in charge of that supplier has to chase them up.
* The order books sent out weekly are numbered or labelled accordingly e.g. 'Week 1' or 'Week of 05-08-2019'
* If the supplier fails to send the order book back by the deadline Monday lunchtime, then the buyers are usually responsible for chasing them up. The users present agree that automatic email reminders would be useful (they send them out manually at the moment) but the frequency of these reminders varies based on the buyer and the supplier.
    * Richard Jordan suggests once every two hours from the lunchtime deadline. Most users at this URC said that they would remind the supplier on Monday afternoon, then Tuesday morning, then they ring them. There should probably be some flexibility to satisfy both types - to be discussed.
    * These reminders say something along the lines of, 'Why haven't you responded? Please respond by Monday lunchtime.'
* Some Order Books don't go directly to the supplier. Instead they are sent to the buyer in charge who decides whether to forward it on.
* For STI and Axiom, these Order Books are first sent to DA (Kerrie Pyburn), who customises them to how the buyer wants them to look. However, one of the aims of the new solution should be to standardise the format across the board.
* The majority of suppliers are sent the 'full' order book every week (but what is 'full' in this context?)
    * For some major or key suppliers, the 'full' order book is only sent out on the first week of every month. For subsequent weeks a 'partial' order book which only shows the orders for a limited time frame is issued. This is due to there being so many orders from some suppliers that it becomes unmanageable to complete a 'full' order book every week.
    * Which suppliers get 'full' or 'partial' order books is subjectively by the buyer, rather than an automated assessment of 'does this supplier's Order Book have more than a certain number of lines?' This should not change under the new system.

#### Kim Legg's Suggestions
* There is too much human intervention at the moment. What if we had nobody running the order books, nobody pressing a button or intervening? Instead, it runs automatically overnight, copying DA and Buyers into the email that is sent out to suppliers.
* As the responses are received, we should also populate a separate spreadsheet that colour codes supplier response time e.g. green for on-time, orange if the buyer had to chase them for their response, or red for no response.
* Issues and delivery date changes too should be colour coordinated, possibly according to severity e.g. purple for a catastrophic issue.
* If a delivery date changes, the system should auto-populate an email to the buyer responsible for that supplier saying, for example, 'Are you aware STI has changed this date?'
* We should reduce the copy/paste requirement, potentially by introducing a portal, and strive for a clean presentation of results and responses on a web page. You (as a buyer) should be able to see only the information or suppliers that relate to you.
* Kerrie is currently the main user of expediting of reports; Kim used to do expediting of reports. She thinks that reports are too complicated, some details could be dropped to give just the important information, including supplier name, order name, expected delivery dates, etc...
* Moreover, at the moment there are some columns bespoke to each supplier; this format could be standardised. We should ask ourselves, for each supplier, are there any specifics bespoke to that supplier? Then, we should get a general consensus that everyone is asking for the same things in different words.
* After we get the foundation/report to what we want, *then* we can refine the process, asking, 'What is key information required from each supplier?' We can use this info to tweak individual reports for each supplier if required.

#### Other Suggestions
* Ability to download order book and print it off is required.
* The response sheet and Order Book Status table should have reason codes for date changes.
* There are two 'statuses' - one is whether the supplier has responded in full, another is a summary of the issues or date changes, or a line by line of the order book.
* A summary of the issues, and then double click or drill down to get full detail.

#### Ubiquitious Language
* Order Book - This refers to the solution currently in place, as well as the Excel spreadsheet that is sent to each supplier. These will continue to be called order books under the new system because that is what suppliers are used to, so it is important to avoid confusion.

## Actions
|Ref|Description|Status|Owner
|---|---|---|---|
|1|Understand the difference between direct and indirect buyers and how this affects user roles and involvement with the order books/system.|Pending|Dev Team|
|2|Clarify how often 'full' or 'partial' order books are sent out, and how long the window is for each e.g. do full order books show all orders for a supplier or just all orders within six weeks?|20/08/2019|Dev Team|
|3|Create storyboards and present these at a Storyboard Review meeting.|20/08/2019|Dev Team|
|4|What does the 'Z' in 'ZLTA' (... Long Term Agreement) stand for?|20/08/2019|Dev Team|
|5|Append user stories on Impact Map for assessement at Impact Map Refinement Event|N/A|Dev Team|
|6|Hold Impact Map Refinement Event|N/A|Dev Team| 

## Outputs
#### Buyer Template
![](http://ggbpla0i:3638/sites/DevOpsProjects/Delivery%20Management%20Tool/Shared%20Documents/URC_Workshop_Buyer_Template.JPG)

#### Delivery Assurance Template
![](http://ggbpla0i:3638/sites/DevOpsProjects/Delivery%20Management%20Tool/Shared%20Documents/URC_Workshop_Delivery_Assurance_Template.JPG)

#### Roles
![](http://ggbpla0i:3638/sites/DevOpsProjects/Delivery%20Management%20Tool/Shared%20Documents/URC_Workshop_Roles.JPG)

#### UI Flow
![](http://ggbpla0i:3638/sites/DevOpsProjects/Delivery%20Management%20Tool/Shared%20Documents/URC_Workshop_UI_Flow.JPG)

#### Wire Frame
![](http://ggbpla0i:3638/sites/DevOpsProjects/Delivery%20Management%20Tool/Shared%20Documents/URC_Workshop_Wire_Frame.JPG)