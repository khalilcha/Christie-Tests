**Build/Run requirements**

The project is made with visual studio 2019 (community), C# language, and targets .net framework 4.7.2. 

The test code is all in `ChristieTests.cs`

Selenium, chromedriver and other packages should be automatically pulled in by nuget on build
Chrome driver package is version 88.0.4324.96, make sure your machine has correct chrome version (chrome 88) installed.

Run the tests by opening project in visual studio, then opening the Test explorer from the test menu in visual studio. Run desired tests from test explorer.

**Test Scenarios**

This exercise is to increase test coverage by writing some tests for the clearance projectors page. We are to write 5 scenarios, and automate 3 of them. By examining the page we can develop various test scenarios. 

Here is the page: https://christieclearanceprojectors.com/collections/clearance-projectors

The page presents a grid of projectors, with their names, prices, and an image of the projector. Each one of these grid items is clickable and should take the user to another webpage with more information about that projector, purchasing information, etc. 

An obvious failure case is that a projector on the grid does not behave in the expected way. Either it is not clickable, or it is clickable, but does not take the user to the appropriate destination (eg clicking takes you to a 404 page or the page for a different projector). This leads to our first test case.

---

<u>First test Scenario</u> (Chosen for automation, TestProjectorClick)

We are to make sure that every projector on the clearance page grid:

* has a link and can be clicked

* takes the user to the page for the correct projector when clicked 

We can validate the latter requirement by checking the projector title & price information on the new page, and then comparing it to the title/price information found in the clicked grid item.

We also must make sure to check every projector on all pages of the grid 

This is one of our automated tests, and at the time of writing it is currently passing.

---

The clearance page also has two drop down menus the user can interact with. One menu labelled "Sort By", another labelled "Browse By". The "Sort by" menu changes the order of what is in the grid based on whatever menu item is selected (price, alphabetical...etc). The "Browse By" menu filters elements from the grid, by default it is set to show "All" projectors. If a user selects "Used" in browse by it will update the grid to only show projectors that are "used" and no other projectors should be visible.

We can create several test scenarios for user interaction with these two menus as these are the main two inputs on the page.

---

<u>Second Test Scenario</u> (Chosen for automation, TestBrowseByUsed)

We are to make sure that selection of the "Browse By" option "Used" updates the grid and:

* The new grid of projectors is a subset of the grid of all projectors, that is to say, there should be no projectors here that were not also visible on the all projectors page

* All projectors on this page are marked "used", there should not be any other type of projector in the grid, for example "Certified Refurbished" which is a different category

Again all pages should be checked if we have multiple pages of projectors.

Note: This is one of the automated tests and the automated test currently fails. It fails due to 2 main problems:

On the "all" projectors page there is a projector named `Captiva DUW350S 1DLP laser projector - Used`. We expect this projector to also be in the new grid when we select "Browse By used" as it is marked use. However unlike the other projectors that are marked used, this projector does not appear as expected on the Used projectors page.

Another issue is that when we select "Browse by used", one of the items is `LW502 3LCD Projector - Certified Refurbished`. This projector is not marked used, and there is no other "Certified Refurbished" projector in the grid. This projector should not be present based on our criteria.

As such this test is currently failing.

---

<u>Third Test Scenario</u> (Chosen for automation, TestSortPriceAscending)

We are to make sure that selection of the "Sort By" option "Price, low to high" updates the grid and:

* We receive a new grid with the exact same elements, except that the order is different. They should now be in ascending order of the price.

* For projectors on sale, that have a sale price (what a customer would actually pay), and a regular price (that is marked down), only the sale price is considered for the order

* No specific behavior assumed/required for the ordering of two elements that have the same price

Again all pages should be checked if we have multiple pages of projectors.

This is one of our automated tests, and at the time of writing it is currently passing.

---

<u>Fourth Test Scenario</u> (Not automated)

In this scenario we are to test that "Sort By" and "Browse By" can work simultaneously without issue. 

An example of this case would be Simultaneously selecting both "Browse by Used" and "Sort by Price, low to high". The resulting grid should have a combination of requirements from the third and second test cases above, as well as some additional criteria.

So the criteria would be:

* The order of operations should not matter. That is to say, if we select "Browse by Used" first, and then "Sort by Price, low to high" after that, the result should be identical to when we select "Sort by Price, low to high" first and "Browse by Used" second.

* The new grid of projectors is a subset of the grid of all projectors, that is to say, there should be no projectors here that were not also visible on the all projectors page

* All projectors on this page are marked "used", there should not be any other type of projector in the grid like "Certified Refurbished" which is a different category

* The elements in this grid should be the same as if we only selected the "browse by used" option except for the order in which they are arranged. They should be in ascending order of the price.

* For projectors on sale, that have a sale price (what a customer would actually pay), and a regular price (that is marked down), only the sale price is considered for the order

* No specific behavior assumed/required for the ordering of two elements that have the same price

Again all pages should be checked if we have multiple pages of projectors.

---

<u>Fifth test scenario</u> (Not automated)

In this scenario, we are to make sure we test that the default selection on the clearance page for the option "browse by" is set to "all", as it may be counterintuitive to open the page for clearance projectors and only receive, say, 1DLP projectors. Se we are to validate

*  "browse by" is set to "all" by default when the page is loaded

We could also try and make sure the content displayed for "all" is correct. One way to do this would be to select the other options in the dropdowns, and make sure their results are all strictly subsets of the content provided by the "all" page. This would make sure that there are no hidden items that are not visible from "all" but visible from one of the other options. However this may not be necessary. In our second, third and fourth scenarios above, we already test to make sure the results are strictly subsets of the "all" page. If we had written tests for all the options under "browse by" and "sort by" with this criteria, then we would have already validated this.

---

