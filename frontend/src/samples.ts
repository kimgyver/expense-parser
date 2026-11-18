export const samples = {
  success: `Hi Patricia,
Please create an expense claim for the below. Relevant details are marked
up as requested…

<expense><cost_centre>DEV632</cost_centre><total>35,000</total><payment_method>personal
card</payment_method></expense>
From: William Steele
Sent: Friday, 16 June 2022 10:32 AM
To: Maria Washington
Subject: test
Hi Maria,
Please create a reservation for 10 at the <vendor>Seaside Steakhouse</vendor> for our
<description>development team's project end celebration</description> on <date>27 April
2022</date> at 7.30pm.
Regards,
William`,

  missingTotal: `Please book dinner.

<vendor>The Grill House</vendor>
<description>Team catch-up</description>
<date>3 May 2022</date>

<expense>
  <cost_centre>OPS200</cost_centre>
  <payment_method>personal card</payment_method>
</expense>`,

  unbalancedTag: `Hi Maria,

Reservation at <vendor>Seaside Steakhouse for the team on <date>27 April 2022</date>.

Also expense:
<expense><cost_centre>DEV632</cost_centre><total>350</total></expense>`,
} as const

export type SampleKey = keyof typeof samples
