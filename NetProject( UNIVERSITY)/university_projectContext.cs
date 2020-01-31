using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace NetProject__UNIVERSITY_
{
    public partial class university_projectContext : DbContext
    {
        public university_projectContext()
        {
        }

        public university_projectContext(DbContextOptions<university_projectContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Courses> Courses { get; set; }
        public virtual DbSet<CoursesMark> CoursesMark { get; set; }
        public virtual DbSet<CriteriaMark> CriteriaMark { get; set; }
        public virtual DbSet<Criterias> Criterias { get; set; }
        public virtual DbSet<DepartmentNews> DepartmentNews { get; set; }
        public virtual DbSet<Faculties> Faculties { get; set; }
        public virtual DbSet<FacultyNews> FacultyNews { get; set; }
        public virtual DbSet<Lecturers> Lecturers { get; set; }
        public virtual DbSet<LecturersMark> LecturersMark { get; set; }
        public virtual DbSet<MarkingDate> MarkingDate { get; set; }
        public virtual DbSet<Materials> Materials { get; set; }
        public virtual DbSet<Publications> Publications { get; set; }
        public virtual DbSet<SocialNews> SocialNews { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=university_project;Username=postgres;Password=postgres");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Courses>(entity =>
            {
                entity.HasKey(e => e.CourseId);

                entity.ToTable("courses");

                entity.Property(e => e.CourseId).HasColumnName("course_id");

                entity.Property(e => e.CourseDescription).HasColumnName("course_description");

                entity.Property(e => e.CourseLink).HasColumnName("course_link");

                entity.Property(e => e.DateId).HasColumnName("date_id");

                entity.Property(e => e.FacultyId).HasColumnName("faculty_id");

                entity.Property(e => e.Literature).HasColumnName("literature");

                entity.Property(e => e.Materials).HasColumnName("materials");

                entity.Property(e => e.Plan).HasColumnName("plan");

                entity.HasOne(d => d.Date)
                    .WithMany(p => p.Courses)
                    .HasForeignKey(d => d.DateId)
                    .HasConstraintName("courses_date_id_fkey");

                entity.HasOne(d => d.Faculty)
                    .WithMany(p => p.Courses)
                    .HasForeignKey(d => d.FacultyId)
                    .HasConstraintName("courses_faculty_id_fkey");
            });

            modelBuilder.Entity<CoursesMark>(entity =>
            {
                entity.HasKey(e => e.CourseMarkId);

                entity.ToTable("courses_mark");

                entity.Property(e => e.CourseMarkId).HasColumnName("course_mark_id");

                entity.Property(e => e.CourseId).HasColumnName("course_id");

                entity.Property(e => e.CourseMark)
                    .HasColumnName("course_mark")
                    .HasColumnType("numeric(5,2)");

                entity.Property(e => e.DescriptionMark)
                    .HasColumnName("description_mark")
                    .HasColumnType("numeric(5,2)");

                entity.Property(e => e.MaterialsMark)
                    .HasColumnName("materials_mark")
                    .HasColumnType("numeric(5,2)");

                entity.Property(e => e.PlanMark)
                    .HasColumnName("plan_mark")
                    .HasColumnType("numeric(5,2)");

                entity.HasOne(d => d.Course)
                    .WithMany(p => p.CoursesMark)
                    .HasForeignKey(d => d.CourseId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("courses_mark_course_id_fkey");
            });

            modelBuilder.Entity<CriteriaMark>(entity =>
            {
                entity.ToTable("criteria_mark");

                entity.Property(e => e.CriteriaMarkId).HasColumnName("criteria_mark_id");

                entity.Property(e => e.CriteriaId).HasColumnName("criteria_id");

                entity.Property(e => e.DateId).HasColumnName("date_id");

                entity.Property(e => e.FacultyId).HasColumnName("faculty_id");

                entity.Property(e => e.Mark)
                    .HasColumnName("mark")
                    .HasColumnType("numeric(5,2)");

                entity.HasOne(d => d.Criteria)
                    .WithMany(p => p.CriteriaMark)
                    .HasForeignKey(d => d.CriteriaId)
                    .HasConstraintName("fk_criteria_id");

                entity.HasOne(d => d.Date)
                    .WithMany(p => p.CriteriaMark)
                    .HasForeignKey(d => d.DateId)
                    .HasConstraintName("fk_date_id");

                entity.HasOne(d => d.Faculty)
                    .WithMany(p => p.CriteriaMark)
                    .HasForeignKey(d => d.FacultyId)
                    .HasConstraintName("fk_faculty_id");
            });

            modelBuilder.Entity<Criterias>(entity =>
            {
                entity.HasKey(e => e.CriteriaId);

                entity.ToTable("criterias");

                entity.Property(e => e.CriteriaId).HasColumnName("criteria_id");

                entity.Property(e => e.CriteriaName)
                    .HasColumnName("criteria_name")
                    .HasMaxLength(50);

                entity.Property(e => e.Description).HasColumnName("description");
            });

            modelBuilder.Entity<DepartmentNews>(entity =>
            {
                entity.ToTable("department_news");

                entity.Property(e => e.DepartmentNewsId).HasColumnName("department_news_id");

                entity.Property(e => e.DateId).HasColumnName("date_id");

                entity.Property(e => e.DepartmentName).HasColumnName("department_name");

                entity.Property(e => e.DepartmentNewsNumber).HasColumnName("department_news_number");

                entity.Property(e => e.FacultyId).HasColumnName("faculty_id");

                entity.Property(e => e.FiltersNewsNumber).HasColumnName("filters_news_number");

                entity.Property(e => e.Mark)
                    .HasColumnName("mark")
                    .HasColumnType("numeric(5,2)");

                entity.HasOne(d => d.Date)
                    .WithMany(p => p.DepartmentNews)
                    .HasForeignKey(d => d.DateId)
                    .HasConstraintName("department_news_date_id_fkey");

                entity.HasOne(d => d.Faculty)
                    .WithMany(p => p.DepartmentNews)
                    .HasForeignKey(d => d.FacultyId)
                    .HasConstraintName("department_news_faculty_id_fkey");
            });

            modelBuilder.Entity<Faculties>(entity =>
            {
                entity.HasKey(e => e.FacultyId);

                entity.ToTable("faculties");

                entity.Property(e => e.FacultyId).HasColumnName("faculty_id");

                entity.Property(e => e.FacultyLanguage)
                    .HasColumnName("faculty_language")
                    .HasMaxLength(10);

                entity.Property(e => e.FacultyLink)
                    .HasColumnName("faculty_link")
                    .HasMaxLength(50);

                entity.Property(e => e.FacultyName)
                    .HasColumnName("faculty_name")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<FacultyNews>(entity =>
            {
                entity.ToTable("faculty_news");

                entity.Property(e => e.FacultyNewsId).HasColumnName("faculty_news_id");

                entity.Property(e => e.DateId).HasColumnName("date_id");

                entity.Property(e => e.FacultyId).HasColumnName("faculty_id");

                entity.Property(e => e.Link).HasColumnName("link");

                entity.Property(e => e.Page).HasColumnName("page");

                entity.Property(e => e.PostingDate)
                    .HasColumnName("posting_date")
                    .HasColumnType("date");

                entity.HasOne(d => d.Date)
                    .WithMany(p => p.FacultyNews)
                    .HasForeignKey(d => d.DateId)
                    .HasConstraintName("faculty_news_date_id_fkey");

                entity.HasOne(d => d.Faculty)
                    .WithMany(p => p.FacultyNews)
                    .HasForeignKey(d => d.FacultyId)
                    .HasConstraintName("faculty_news_faculty_id_fkey");
            });

            modelBuilder.Entity<Lecturers>(entity =>
            {
                entity.HasKey(e => e.LecturerId);

                entity.ToTable("lecturers");

                entity.Property(e => e.LecturerId).HasColumnName("lecturer_id");

                entity.Property(e => e.AcademicStatus).HasColumnName("academic_status");

                entity.Property(e => e.Biography).HasColumnName("biography");

                entity.Property(e => e.Contact).HasColumnName("contact");

                entity.Property(e => e.DateId).HasColumnName("date_id");

                entity.Property(e => e.FacultyId).HasColumnName("faculty_id");

                entity.Property(e => e.Link).HasColumnName("link");

                entity.Property(e => e.Name).HasColumnName("name");

                entity.Property(e => e.Position).HasColumnName("position");

                entity.Property(e => e.PublicationId).HasColumnName("publication_id");

                entity.Property(e => e.ScientificInterests).HasColumnName("scientific_interests");

                entity.HasOne(d => d.Date)
                    .WithMany(p => p.Lecturers)
                    .HasForeignKey(d => d.DateId)
                    .HasConstraintName("lecturers_date_id_fkey");

                entity.HasOne(d => d.Faculty)
                    .WithMany(p => p.Lecturers)
                    .HasForeignKey(d => d.FacultyId)
                    .HasConstraintName("lecturers_faculty_id_fkey");

                entity.HasOne(d => d.Publication)
                    .WithMany(p => p.Lecturers)
                    .HasForeignKey(d => d.PublicationId)
                    .HasConstraintName("lecturers_publication_id_fkey");
            });

            modelBuilder.Entity<LecturersMark>(entity =>
            {
                entity.ToTable("lecturers_mark");

                entity.Property(e => e.LecturersMarkId).HasColumnName("lecturers_mark_id");

                entity.Property(e => e.BiographyMark)
                    .HasColumnName("biography_mark")
                    .HasColumnType("numeric(5,2)");

                entity.Property(e => e.ContactMark)
                    .HasColumnName("contact_mark")
                    .HasColumnType("numeric(5,2)");

                entity.Property(e => e.HyperlinkMark)
                    .HasColumnName("hyperlink_mark")
                    .HasColumnType("numeric(5,2)");

                entity.Property(e => e.LecturerId).HasColumnName("lecturer_id");

                entity.Property(e => e.LecturerMark)
                    .HasColumnName("lecturer_mark")
                    .HasColumnType("numeric(5,2)");

                entity.Property(e => e.NameMark)
                    .HasColumnName("name_mark")
                    .HasColumnType("numeric(5,2)");

                entity.Property(e => e.PublicationMark)
                    .HasColumnName("publication_mark")
                    .HasColumnType("numeric(5,2)");

                entity.Property(e => e.SchientificInterestsMark)
                    .HasColumnName("schientific_interests_mark")
                    .HasColumnType("numeric(5,2)");

                entity.HasOne(d => d.Lecturer)
                    .WithMany(p => p.LecturersMark)
                    .HasForeignKey(d => d.LecturerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("lecturers_mark_lecturer_id_fkey");
            });

            modelBuilder.Entity<MarkingDate>(entity =>
            {
                entity.HasKey(e => e.DateId);

                entity.ToTable("marking_date");

                entity.Property(e => e.DateId).HasColumnName("date_id");

                entity.Property(e => e.Date).HasColumnName("date");
            });

            modelBuilder.Entity<Materials>(entity =>
            {
                entity.ToTable("materials");

                entity.Property(e => e.MaterialsId).HasColumnName("materials_id");

                entity.Property(e => e.DateId).HasColumnName("date_id");

                entity.Property(e => e.DepartmentName).HasColumnName("department_name");

                entity.Property(e => e.FacultyId).HasColumnName("faculty_id");

                entity.Property(e => e.Mark)
                    .HasColumnName("mark")
                    .HasColumnType("numeric(5,2)");

                entity.Property(e => e.MaterialsLinks).HasColumnName("materials_links");

                entity.Property(e => e.MaterialsNumber).HasColumnName("materials_number");

                entity.HasOne(d => d.Date)
                    .WithMany(p => p.Materials)
                    .HasForeignKey(d => d.DateId)
                    .HasConstraintName("materials_date_id_fkey");

                entity.HasOne(d => d.Faculty)
                    .WithMany(p => p.Materials)
                    .HasForeignKey(d => d.FacultyId)
                    .HasConstraintName("materials_faculty_id_fkey");
            });

            modelBuilder.Entity<Publications>(entity =>
            {
                entity.HasKey(e => e.PublicationId);

                entity.ToTable("publications");

                entity.Property(e => e.PublicationId).HasColumnName("publication_id");

                entity.Property(e => e.Hyperlink).HasColumnName("hyperlink");

                entity.Property(e => e.HyperlinkNumber).HasColumnName("hyperlink_number");

                entity.Property(e => e.Publication).HasColumnName("publication");

                entity.Property(e => e.PublicationNumber).HasColumnName("publication_number");
            });

            modelBuilder.Entity<SocialNews>(entity =>
            {
                entity.ToTable("social_news");

                entity.Property(e => e.SocialNewsId).HasColumnName("social_news_id");

                entity.Property(e => e.DateId).HasColumnName("date_id");

                entity.Property(e => e.FacultyId).HasColumnName("faculty_id");

                entity.Property(e => e.LastPostDate)
                    .HasColumnName("last_post_date")
                    .HasColumnType("date");

                entity.Property(e => e.PostNumbers).HasColumnName("post_numbers");

                entity.HasOne(d => d.Date)
                    .WithMany(p => p.SocialNews)
                    .HasForeignKey(d => d.DateId)
                    .HasConstraintName("social_news_date_id_fkey");

                entity.HasOne(d => d.Faculty)
                    .WithMany(p => p.SocialNews)
                    .HasForeignKey(d => d.FacultyId)
                    .HasConstraintName("social_news_faculty_id_fkey");
            });
        }
    }
}
