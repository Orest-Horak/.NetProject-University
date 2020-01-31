using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetProject__UNIVERSITY_.Models.Comparers
{
    public class FacultyNewsComparer : IEqualityComparer<FacultyNews>
    {
        public bool Equals(FacultyNews x, FacultyNews y)
        {

            //Check whether the compared objects reference the same data.
            if (Object.ReferenceEquals(x, y)) return true;

            //Check whether any of the compared objects is null.
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            //Check whether the products' properties are equal.
            return x.Link == y.Link;
        }

        public int GetHashCode(FacultyNews facultyNews)
        {
            //Check whether the object is null
            if (Object.ReferenceEquals(facultyNews, null)) return 0;

            //Get hash code for the Link field if it is not null.
            int hashLink = facultyNews.Link == null ? 0 : facultyNews.Link.GetHashCode();

            //Calculate the hash code for the facultyNews.
            return hashLink;
        }
    }
}
