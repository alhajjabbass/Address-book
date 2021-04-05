# Address Book Web Service
##### Built using .Net Core

## Notes

- I built it using .Net Core because I am most comfortable with that technology during this phase in my career, and it would be the best technology for me to achieve the most features.
- PostgreSQL as a database, I chose PSQL because of the relational requirement needed in this project, meant it was better than using a NO-SQL DB like MongoDB. (needed some learning but I figured it out :D)
- I used generatedata.com to generate mock data via a query into my database table. check [filldata.txt](https://github.com/alhajjabbass/Address-book/blob/master/filldata.txt)


## Features Implemented

- **API design that adheres to standards**: as my first real developled API I think I have done quite ok :D
- **API to create, update and delete entries**: check [API Documentation](https://github.com/alhajjabbass/Address-book/blob/master/AnghamiApiDocumentation.docx)
- **Ability to group entries by job title, location and age**: Made it as an API to be called from the frontend. check [API Documentation](https://github.com/alhajjabbass/Address-book/blob/master/AnghamiApiDocumentation.docx)
- **Paginated lists**: done through the API by adding *"page"* to the request.
- **Caching**: Used caching in one of the functions which returns the people from the DB.
- **Ability to search for people**: You can search for people through one or more of these filters:
    - firstname
    - lastname
    - age
    - email
    - phone
- **Testing**: Added one unit test for one function as a POC.

## Features NOT Implemented :(
Below are the features I did not have time to implement due to the timing constraint and not knowing the basic of a UI framework (React).
- **Add relationships between people**
- **Asynchronous processing or adding profile photo**
- **Use of a UI framework to showcase the data**: Depends on my ability to create a sample UI project by today :D
- Implementing a better way for caching.
