# Portfolio Creator

Hello!
This is my project with which I try to show what I have learned so far in web application development with c# at the Softuni academy!

- [Description](#description)
- [Features](#features)
- [License](#license)
- [Contact](#contact)

## Run Information

- to seed the database it is necessary to uncomment the dataseeder in ApplicationDbContext - OnModelCreating() method <br />
var seeder = new DataSeeder(_env); <br />
seeder.Seed(builder);
- this method is just for database seeding and must be excluded again after creating of the database is done
- for some functions is needed running Application.Api
- the files in namespace Application.Data.Migrations are excluded from the Visual Studio's code coverage analysis

## Description

This is a user-friendly ASP.NET web application that allows individuals to create and showcase their personalized portfolios online. The application's main function is to provide users with a platform to display their skills, experiences, and projects in a professional and visually appealing manner.

## Features

- User Registration and Authentication: New users can create an account by registering with a unique username and password.
- Portfolio Creation: Once logged in, users can easily create their portfolios.
- Project Showcase: Users can add projects they have worked on, along with descriptions, images, or links to external sources to showcase their work and skills effectively.
- Experience and Education: The application allows users to add their work experience and educational background, giving visitors a comprehensive view of their professional journey.
- Responsive Design: The application is designed with a responsive layout, ensuring that portfolios can be easily viewed on different devices, such as desktops, tablets, and smartphones.
- Easy Editing: Users can edit their portfolios at any time, adding new projects, updating information etc.
- Share and Export: Users can share their portfolios with others through direct links or social media, as well as export their portfolios as PDF files for offline use or printing.
- enables visitors to send direct messages to the owner of a portfolio. This functionality facilitates communication between potential clients, employers, or collaborators and the portfolio owner, fostering meaningful interactions and opportunities

## License
Open source

## Contact
nikolaytotuhov@gmail.com
