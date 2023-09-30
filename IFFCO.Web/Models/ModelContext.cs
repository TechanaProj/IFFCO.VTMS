using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Devart.Data.Oracle;
using IFFCO.HRMS.Entities.AppConfig;
using IFFCO.HRMS.Repository.Pattern;
using IFFCO.HRMS.Repository.Pattern.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace IFFCO.VTMS.Web.Models
{
    public partial class ModelContext : DbContext
    {
        readonly string conn = new AppConfiguration().ConnectionString;
        readonly string SchemaName = new AppConfiguration().SchemaName;

        public ModelContext()
        {
        }

        public ModelContext(DbContextOptions<ModelContext> options)
            : base(options)
        {
        }

        public virtual DbSet<MDistrict> MDistrict { get; set; }
        public virtual DbSet<MState> MState { get; set; }
        public virtual DbSet<VtmsBranchMsts> VtmsBranchMsts { get; set; }
        public virtual DbSet<VtmsCourseMsts> VtmsCourseMsts { get; set; }
        public virtual DbSet<VtmsEnrollDoc> VtmsEnrollDoc { get; set; }
        public virtual DbSet<VtmsEnrollEdu> VtmsEnrollEdu { get; set; }
        public virtual DbSet<VtmsEnrollPi> VtmsEnrollPi { get; set; }
        public virtual DbSet<VtmsEvalDtl> VtmsEvalDtl { get; set; }
        public virtual DbSet<VtmsEvalMsts> VtmsEvalMsts { get; set; }
        public virtual DbSet<VtmsInstituteMsts> VtmsInstituteMsts { get; set; }
        public virtual DbSet<VtmsRecommMsts> VtmsRecommMsts { get; set; }
        public virtual DbSet<VtmsUniversityMsts> VtmsUniversityMsts { get; set; }
        public virtual DbSet<VtmsVtReview> VtmsVtReview { get; set; }

               
        public DataTable GetSQLQuery(string sqlquery)
        {
            DataTable dt = new DataTable();

            OracleConnection connection = new OracleConnection(conn);

            OracleDataAdapter oraAdapter = new OracleDataAdapter(sqlquery, connection);

            OracleCommandBuilder oraBuilder = new OracleCommandBuilder(oraAdapter);

            oraAdapter.Fill(dt);

            return dt;
        }

        public int insertUpdateToDB(string sql)
        {
            OracleConnection connection = new OracleConnection(conn);
            OracleCommand cmd = new OracleCommand();
            int i = 0;
            try
            {
                cmd.CommandText = sql.ToString();
                cmd.Connection = connection;
                connection.Open();
                i = cmd.ExecuteNonQuery();
                connection.Close();
                return i;
            }
            catch (Exception ex)
            {
                var Message = ex.Message;
                return i = 0;
            }
        }

        public int GetScalerFromDB(string sql)
        {
            OracleConnection connection = new OracleConnection(conn);
            OracleCommand cmd = new OracleCommand();
            int i = 0;
            try
            {
                cmd.CommandText = sql.ToString();
                cmd.Connection = connection;
                connection.Open();
                int result = Convert.ToInt32(cmd.ExecuteScalar());
                connection.Close();
                return result;
            }
            catch (Exception ex)
            {
                var Message = ex.Message;
                return i = 0;
            }
        }

        public string GetCharScalerFromDB(string sql)
        {
            OracleConnection connection = new OracleConnection(conn);
            OracleCommand cmd = new OracleCommand();
            int i = 0;
            try
            {
                cmd.CommandText = sql.ToString();
                cmd.Connection = connection;
                connection.Open();
                string result = Convert.ToString(cmd.ExecuteScalar());
                connection.Close();
                return result;
            }
            catch (Exception ex)
            {
                var Message = ex.Message;
                return null;
            }
        }

        public int ExecuteProcedure(string procedure, params object[] parameters)
        {
            List<OracleParameter> oracleParameterList = ((List<OracleParameter>)parameters[0]);
            string[] oracleParameters = new string[oracleParameterList.Count];
            string query = "BEGIN " + procedure + "(";
            for (int i = 0; i < oracleParameterList.Count; i++)
            {
                OracleParameter parameter = oracleParameterList[i] as OracleParameter;
                oracleParameters[i] = ":" + parameter.ParameterName;
            }
            query += string.Join(",", oracleParameters);
            query += "); end;";
            //Database.OpenConnection()
            return Database.ExecuteSqlCommand(query, oracleParameterList);
        }



        public int ExecuteProcedureForRefCursor(string procedure, params object[] parameters)
        {
            List<OracleParameter> oracleParameterList = ((List<OracleParameter>)parameters[0]);
            string[] oracleParameters = new string[oracleParameterList.Count];
            string query = "BEGIN " + procedure + "(";
            for (int i = 0; i < oracleParameterList.Count; i++)
            {
                OracleParameter parameter = oracleParameterList[i] as OracleParameter;
                oracleParameters[i] = ":" + parameter.ParameterName;
            }
            query += string.Join(",", oracleParameters);
            query += "); end;";


            Database.OpenConnection();
            int x = Database.ExecuteSqlCommand(query, oracleParameterList);
            return x;
        }

        public Task<int> ExecuteProcedureAsync<TElement>(string procedure, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public int ExecuteQuery<TElement>(string query)
        {
            throw new NotImplementedException();
        }

        public Task<int> ExecuteQueryAsync<TElement>(string query)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TElement> ExecuteSelectProcedure<TElement>(string procedure, params object[] parameters)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TElement> ExecuteSelectQuery<TElement>(string query)
        {
            throw new NotImplementedException();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await this.SaveChangesAsync(CancellationToken.None);
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            //SyncObjectsStatePreCommit();
            var changesAsync = await base.SaveChangesAsync(cancellationToken);
            SyncObjectsStatePostCommit();
            return changesAsync;
        }

        public void SyncObjectsStatePostCommit()
        {
            foreach (var dbEntityEntry in ChangeTracker.Entries())
            {
                dbEntityEntry.State = StateHelper.ConvertState(((IObjectState)dbEntityEntry.Entity).ObjectState);
            }
        }
        private void SyncObjectsStatePreCommit()
        {
            foreach (var dbEntityEntry in ChangeTracker.Entries())
            {
                dbEntityEntry.State = StateHelper.ConvertState(((IObjectState)dbEntityEntry.Entity).ObjectState);
            }
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                //  optionsBuilder.UseOracle("SERVICE NAME=ORACLE;Direct=true; License Key=vEyr8GdnIarOKlEHQKxi+4E0HlXN85PVGHI096M18fHO05syciZT/8xvOeNbTwuMbqdZkRZ1qbdPjO13mrnBnlMUyskKr9qbBNMTzJAp5+R858T7YUZaTY5rodcDl7pDutJBeuYiwHG+xtXnywKMPX+9u82fR1AMT9EailpEiBp1OAn6IbJ55eXY15+rsAfDDwUuIv/js610S6cy9vLt37IL4PcZ8Wx/MrQlA38Z+kEH9Wztcv+NSWFVRz2wnVRDtIowaySSKk30sA+MBbg2IIUI+/MgDUp6w53NCxQSsuM=; User Id=IDEA_FRI;Password=idea_fri; Data Source= 10.80.1.8:1521/iffcoal.ifdbmumprvtsbnt.ifmumvcn.oraclevcn.com;");
                optionsBuilder.UseOracle(conn);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MDistrict>(entity =>
            {
                entity.HasKey(e => e.DisttCd);

                entity.ToTable("M_DISTRICT", "IDEA_FRI");

                entity.HasIndex(e => e.DisttCd)
                    .HasName("M_DISTRICT_PK")
                    .IsUnique();

                entity.Property(e => e.DisttCd)
                    .HasColumnName("DISTT_CD")
                    .HasColumnType("char")
                    .HasMaxLength(6);

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("CREATED_BY")
                    .HasColumnType("varchar2")
                    .HasMaxLength(15);

                entity.Property(e => e.CreationDate)
                    .HasColumnName("CREATION_DATE")
                    .HasColumnType("date");

                entity.Property(e => e.DisttName)
                    .HasColumnName("DISTT_NAME")
                    .HasColumnType("varchar2")
                    .HasMaxLength(40);

                entity.Property(e => e.ModificationDt)
                    .HasColumnName("MODIFICATION_DT")
                    .HasColumnType("date");

                entity.Property(e => e.ModifiedBy)
                    .HasColumnName("MODIFIED_BY")
                    .HasColumnType("varchar2")
                    .HasMaxLength(15);

                entity.Property(e => e.StateCd)
                    .HasColumnName("STATE_CD")
                    .HasColumnType("char")
                    .HasMaxLength(2);

                entity.Property(e => e.Status)
                    .HasColumnName("STATUS")
                    .HasColumnType("char")
                    .HasMaxLength(1);
            });

            modelBuilder.Entity<MState>(entity =>
            {
                entity.HasKey(e => e.StateCd);

                entity.ToTable("M_STATE", "IDEA_FRI");

                entity.HasIndex(e => e.StateCd)
                    .HasName("M_STATE_PK")
                    .IsUnique();

                entity.Property(e => e.StateCd)
                    .HasColumnName("STATE_CD")
                    .HasColumnType("char")
                    .HasMaxLength(2);

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("CREATED_BY")
                    .HasColumnType("varchar2")
                    .HasMaxLength(15);

                entity.Property(e => e.CreationDate)
                    .HasColumnName("CREATION_DATE")
                    .HasColumnType("date");

                entity.Property(e => e.IsoStateCd)
                    .HasColumnName("ISO_STATE_CD")
                    .HasColumnType("varchar2")
                    .HasMaxLength(10);

                entity.Property(e => e.IsoStateName)
                    .HasColumnName("ISO_STATE_NAME")
                    .HasColumnType("varchar2")
                    .HasMaxLength(30);

                entity.Property(e => e.ModificationDt)
                    .HasColumnName("MODIFICATION_DT")
                    .HasColumnType("date");

                entity.Property(e => e.ModifiedBy)
                    .HasColumnName("MODIFIED_BY")
                    .HasColumnType("varchar2")
                    .HasMaxLength(15);

                entity.Property(e => e.StateName)
                    .HasColumnName("STATE_NAME")
                    .HasColumnType("varchar2")
                    .HasMaxLength(40);

                entity.Property(e => e.Status)
                    .HasColumnName("STATUS")
                    .HasColumnType("char")
                    .HasMaxLength(1);
            });

            modelBuilder.Entity<VtmsBranchMsts>(entity =>
            {
                entity.HasKey(e => e.BranchId);

                entity.ToTable("VTMS_BRANCH_MSTS", "IDEA_FRI");

                entity.HasIndex(e => e.BranchId)
                    .HasName("SYS_C00257182")
                    .IsUnique();

                entity.Property(e => e.BranchId).HasColumnName("BRANCH_ID");

                entity.Property(e => e.BranchCode)
                    .HasColumnName("BRANCH_CODE")
                    .HasColumnType("varchar2")
                    .HasMaxLength(40);

                entity.Property(e => e.BranchDesc)
                    .HasColumnName("BRANCH_DESC")
                    .HasColumnType("varchar2")
                    .HasMaxLength(100);

                entity.Property(e => e.CourseCode)
                    .HasColumnName("COURSE_CODE")
                    .HasColumnType("varchar2")
                    .HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasColumnName("CREATED_BY");

                entity.Property(e => e.CreatedDateTime)
                    .HasColumnName("CREATED_DATE_TIME")
                    .HasColumnType("date");
            });

            modelBuilder.Entity<VtmsCourseMsts>(entity =>
            {
                entity.HasKey(e => e.CourseId);

                entity.ToTable("VTMS_COURSE_MSTS", "IDEA_FRI");

                entity.HasIndex(e => e.CourseId)
                    .HasName("SYS_C00257183")
                    .IsUnique();

                entity.Property(e => e.CourseId).HasColumnName("COURSE_ID");

                entity.Property(e => e.CourseCode)
                    .HasColumnName("COURSE_CODE")
                    .HasColumnType("varchar2")
                    .HasMaxLength(20);

                entity.Property(e => e.CourseDesc)
                    .HasColumnName("COURSE_DESC")
                    .HasColumnType("varchar2")
                    .HasMaxLength(100);

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("CREATED_BY")
                    .HasColumnType("varchar2")
                    .HasMaxLength(30);

                entity.Property(e => e.CreatedDateTime)
                    .HasColumnName("CREATED_DATE_TIME")
                    .HasColumnType("date");
            });

            modelBuilder.Entity<VtmsEnrollDoc>(entity =>
            {
                entity.HasKey(e => e.VtCode);

                entity.ToTable("VTMS_ENROLL_DOC", "IDEA_FRI");

                entity.HasIndex(e => e.VtCode)
                    .HasName("SYS_C00257077")
                    .IsUnique();

                entity.Property(e => e.VtCode)
                    .HasColumnName("VT_CODE")
                    .HasColumnType("varchar2")
                    .HasMaxLength(30);

                entity.Property(e => e.CertFlag)
                    .HasColumnName("CERT_FLAG")
                    .HasColumnType("varchar2")
                    .HasMaxLength(50);

                entity.Property(e => e.DecFlag)
                    .HasColumnName("DEC_FLAG")
                    .HasColumnType("varchar2")
                    .HasMaxLength(50);

                entity.Property(e => e.EnteredBy)
                    .HasColumnName("ENTERED_BY")
                    .HasColumnType("varchar2")
                    .HasMaxLength(50);

                entity.Property(e => e.EnteredOn)
                    .HasColumnName("ENTERED_ON")
                    .HasColumnType("date");

                entity.Property(e => e.ModifiedBy).HasColumnName("MODIFIED_BY");

                entity.Property(e => e.ModifiedOn)
                    .HasColumnName("MODIFIED_ON")
                    .HasColumnType("date");

                entity.Property(e => e.UndertakingFlag)
                    .HasColumnName("UNDERTAKING_FLAG")
                    .HasColumnType("varchar2")
                    .HasMaxLength(250);

                entity.Property(e => e.UnitCode).HasColumnName("UNIT_CODE");

                entity.Property(e => e.VtIdDtl)
                    .HasColumnName("VT_ID_DTL")
                    .HasColumnType("varchar2")
                    .HasMaxLength(50);

                entity.Property(e => e.VtIdType)
                    .HasColumnName("VT_ID_TYPE")
                    .HasColumnType("varchar2")
                    .HasMaxLength(50);

                entity.Property(e => e.VtIdUpload).HasColumnName("VT_ID_UPLOAD");

                entity.Property(e => e.VtPhoto).HasColumnName("VT_PHOTO");
            });

            modelBuilder.Entity<VtmsEnrollEdu>(entity =>
            {
                entity.HasKey(e => e.VtCode);

                entity.ToTable("VTMS_ENROLL_EDU", "IDEA_FRI");

                entity.HasIndex(e => e.VtCode)
                    .HasName("VTMS_ENROLL_EDU_PK")
                    .IsUnique();

                entity.Property(e => e.VtCode)
                    .HasColumnName("VT_CODE")
                    .HasColumnType("varchar2")
                    .HasMaxLength(30);

                entity.Property(e => e.BranchName)
                    .HasColumnName("BRANCH_NAME")
                    .HasColumnType("varchar2")
                    .HasMaxLength(50);

                entity.Property(e => e.CourseName)
                    .HasColumnName("COURSE_NAME")
                    .HasColumnType("varchar2")
                    .HasMaxLength(30);

                entity.Property(e => e.EnrolledBy).HasColumnName("ENROLLED_BY");

                entity.Property(e => e.EnrolledOn)
                    .HasColumnName("ENROLLED_ON")
                    .HasColumnType("date");

                entity.Property(e => e.InstituteName)
                    .HasColumnName("INSTITUTE_NAME")
                    .HasColumnType("varchar2")
                    .HasMaxLength(50);

                entity.Property(e => e.ModifiedBy).HasColumnName("MODIFIED_BY");

                entity.Property(e => e.ModifiedOn)
                    .HasColumnName("MODIFIED_ON")
                    .HasColumnType("date");

                entity.Property(e => e.PrevVtCode)
                    //.IsRequired()
                    .HasColumnName("PREV_VT_CODE")
                    .HasColumnType("varchar2")
                    .HasMaxLength(30);

                entity.Property(e => e.Semester).HasColumnName("SEMESTER");

                entity.Property(e => e.UnitCode).HasColumnName("UNIT_CODE");

                entity.Property(e => e.UniversityName)
                    .HasColumnName("UNIVERSITY_NAME")
                    .HasColumnType("varchar2")
                    .HasMaxLength(50);

                entity.Property(e => e.Year).HasColumnName("YEAR");
            });

            modelBuilder.Entity<VtmsEnrollPi>(entity =>
            {
                entity.HasKey(e => e.VtCode);

                entity.ToTable("VTMS_ENROLL_PI", "IDEA_FRI");

                entity.HasIndex(e => e.VtCode)
                    .HasName("VTMS_ENROLL_PI_PK")
                    .IsUnique();

                entity.Property(e => e.VtCode)
                    .HasColumnName("VT_CODE")
                    .HasColumnType("varchar2")
                    .HasMaxLength(30);

                entity.Property(e => e.Address)
                    .HasColumnName("ADDRESS")
                    .HasColumnType("varchar2")
                    .HasMaxLength(50);

                entity.Property(e => e.ContactNo).HasColumnName("CONTACT_NO");

                entity.Property(e => e.DistrictName)
                    .HasColumnName("DISTRICT_NAME")
                    .HasColumnType("varchar2")
                    .HasMaxLength(50);

                entity.Property(e => e.DocName)
                    .HasColumnName("DOC_NAME")
                    .HasColumnType("varchar2")
                    .HasMaxLength(30);

                entity.Property(e => e.DocRegistrationNo)
                    .HasColumnName("DOC_REGISTRATION_NO")
                    .HasColumnType("varchar2")
                    .HasMaxLength(30);

                entity.Property(e => e.EnrolledBy).HasColumnName("ENROLLED_BY");

                entity.Property(e => e.EnrolledOn)
                    .HasColumnName("ENROLLED_ON")
                    .HasColumnType("date");

                entity.Property(e => e.EnrollmentStatus)
                    .HasColumnName("ENROLLMENT_STATUS")
                    .HasColumnType("varchar2")
                    .HasMaxLength(30);

                entity.Property(e => e.FatherName)
                    .HasColumnName("FATHER_NAME")
                    .HasColumnType("varchar2")
                    .HasMaxLength(30);

                entity.Property(e => e.ManagedBy).HasColumnName("MANAGED_BY");

                entity.Property(e => e.ManagedOn)
                    .HasColumnName("MANAGED_ON")
                    .HasColumnType("date");

                entity.Property(e => e.ModifiedBy).HasColumnName("MODIFIED_BY");

                entity.Property(e => e.ModifiedOn)
                    .HasColumnName("MODIFIED_ON")
                    .HasColumnType("date");

                entity.Property(e => e.Name)
                    .HasColumnName("NAME")
                    .HasColumnType("varchar2")
                    .HasMaxLength(30);

                entity.Property(e => e.OthersRecommName)
                    .HasColumnName("OTHERS_RECOMM_NAME")
                    .HasColumnType("varchar2")
                    .HasMaxLength(30);

                entity.Property(e => e.Pincode).HasColumnName("PINCODE");

                entity.Property(e => e.PrevVtCode)
                    .IsRequired()
                    .HasColumnName("PREV_VT_CODE")
                    .HasColumnType("varchar2")
                    .HasMaxLength(30);

                entity.Property(e => e.RecommPno).HasColumnName("RECOMM_PNO");

                entity.Property(e => e.RecommendationType)
                    .HasColumnName("RECOMMENDATION_TYPE")
                    .HasColumnType("varchar2")
                    .HasMaxLength(30);

                entity.Property(e => e.StateName)
                    .HasColumnName("STATE_NAME")
                    .HasColumnType("varchar2")
                    .HasMaxLength(50);

                entity.Property(e => e.Status)
                    .HasColumnName("STATUS")
                    .HasColumnType("varchar2")
                    .HasMaxLength(30);

                entity.Property(e => e.UnitCode).HasColumnName("UNIT_CODE");

                entity.Property(e => e.VtEndDate)
                    .HasColumnName("VT_END_DATE")
                    .HasColumnType("date");

                entity.Property(e => e.VtStartDate)
                    .HasColumnName("VT_START_DATE")
                    .HasColumnType("date");
            });

            modelBuilder.Entity<VtmsEvalDtl>(entity =>
            {
                entity.HasKey(e => e.EvalId);

                entity.ToTable("VTMS_EVAL_DTL", "IDEA_FRI");

                entity.HasIndex(e => e.EvalId)
                    .HasName("SYS_C00257078")
                    .IsUnique();

                entity.Property(e => e.EvalId)
                    .HasColumnName("EVAL_ID")
                    .HasColumnType("varchar2")
                    .HasMaxLength(15);

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("CREATED_BY")
                    .HasColumnType("varchar2")
                    .HasMaxLength(15);

                entity.Property(e => e.CreationDatetime)
                    .HasColumnName("CREATION_DATETIME")
                    .HasColumnType("date");

                entity.Property(e => e.EvalCategory)
                    .HasColumnName("EVAL_CATEGORY")
                    .HasColumnType("varchar2")
                    .HasMaxLength(100);

                entity.Property(e => e.EvalParameters)
                    .HasColumnName("EVAL_PARAMETERS")
                    .HasColumnType("varchar2")
                    .HasMaxLength(250);

                entity.Property(e => e.Status)
                    .HasColumnName("STATUS")
                    .HasColumnType("varchar2")
                    .HasMaxLength(250);
            });

            modelBuilder.Entity<VtmsEvalMsts>(entity =>
            {
                entity.HasKey(e => e.EvalCatId);

                entity.ToTable("VTMS_EVAL_MSTS", "IDEA_FRI");

                entity.HasIndex(e => e.EvalCatId)
                    .HasName("SYS_C00257084")
                    .IsUnique();

                entity.Property(e => e.EvalCatId)
                    .HasColumnName("EVAL_CAT_ID")
                    .HasColumnType("varchar2")
                    .HasMaxLength(15);

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("CREATED_BY")
                    .HasColumnType("varchar2")
                    .HasMaxLength(15);

                entity.Property(e => e.CreationDatetime)
                    .HasColumnName("CREATION_DATETIME")
                    .HasColumnType("date");

                entity.Property(e => e.EvalCatDesc)
                    .HasColumnName("EVAL_CAT_DESC")
                    .HasColumnType("varchar2")
                    .HasMaxLength(150);

                entity.Property(e => e.Status)
                    .HasColumnName("STATUS")
                    .HasColumnType("varchar2")
                    .HasMaxLength(250);
            });

            modelBuilder.Entity<VtmsInstituteMsts>(entity =>
            {
                entity.HasKey(e => e.InstituteCd);

                entity.ToTable("VTMS_INSTITUTE_MSTS", "IDEA_FRI");

                entity.HasIndex(e => e.InstituteCd)
                    .HasName("VTMS_INSTITUTE_MSTS_PK")
                    .IsUnique();

                entity.Property(e => e.InstituteCd)
                    .HasColumnName("INSTITUTE_CD")
                    .HasColumnType("varchar2")
                    .HasMaxLength(30);

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("CREATED_BY")
                    .HasColumnType("varchar2")
                    .HasMaxLength(50);

                entity.Property(e => e.CreatedDatetime)
                    .HasColumnName("CREATED_DATETIME")
                    .HasColumnType("date");

                entity.Property(e => e.DistrictName)
                    .HasColumnName("DISTRICT_NAME")
                    .HasColumnType("varchar2")
                    .HasMaxLength(50);

                entity.Property(e => e.InstituteName)
                    .HasColumnName("INSTITUTE_NAME")
                    .HasColumnType("varchar2")
                    .HasMaxLength(100);

                entity.Property(e => e.InstituteType)
                    .HasColumnName("INSTITUTE_TYPE")
                    .HasColumnType("varchar2")
                    .HasMaxLength(30);

                entity.Property(e => e.StateName)
                    .HasColumnName("STATE_NAME")
                    .HasColumnType("varchar2")
                    .HasMaxLength(30);

                entity.Property(e => e.UniversityId)
                    .HasColumnName("UNIVERSITY_ID")
                    .HasColumnType("varchar2")
                    .HasMaxLength(30);
            });

            modelBuilder.Entity<VtmsRecommMsts>(entity =>
            {
                entity.HasKey(e => e.RecommId);

                entity.ToTable("VTMS_RECOMM_MSTS", "IDEA_FRI");

                entity.HasIndex(e => e.RecommId)
                    .HasName("VTMS_RECOMM_MSTS_PK")
                    .IsUnique();

                entity.Property(e => e.RecommId)
                    .HasColumnName("RECOMM_ID")
                    .HasColumnType("varchar2")
                    .HasMaxLength(100);

                entity.Property(e => e.RecommName)
                    .HasColumnName("RECOMM_NAME")
                    .HasColumnType("varchar2")
                    .HasMaxLength(100);

                entity.Property(e => e.Status)
                    .HasColumnName("STATUS")
                    .HasColumnType("varchar2")
                    .HasMaxLength(100);

                entity.Property(e => e.UnitCode).HasColumnName("UNIT_CODE");
            });

            modelBuilder.Entity<VtmsUniversityMsts>(entity =>
            {
                entity.HasKey(e => e.UniversityId);

                entity.ToTable("VTMS_UNIVERSITY_MSTS", "IDEA_FRI");

                entity.HasIndex(e => e.UniversityId)
                    .HasName("VTMS_UNIVERSITY_MSTS_PK")
                    .IsUnique();

                entity.Property(e => e.UniversityId).HasColumnName("UNIVERSITY_ID");

                entity.Property(e => e.CreatedBy).HasColumnName("CREATED_BY");

                entity.Property(e => e.CreationDatetime)
                    .HasColumnName("CREATION_DATETIME")
                    .HasColumnType("date");

                entity.Property(e => e.DistrictName)
                    .HasColumnName("DISTRICT_NAME")
                    .HasColumnType("varchar2")
                    .HasMaxLength(100);

                entity.Property(e => e.UniversityName)
                    .HasColumnName("UNIVERSITY_NAME")
                    .HasColumnType("varchar2")
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<VtmsVtReview>(entity =>
            {
                entity.HasKey(e => e.VtCode);

                entity.ToTable("VTMS_VT_REVIEW", "IDEA_FRI");

                entity.HasIndex(e => e.VtCode)
                    .HasName("VTMS_VT_REVIEW_PK")
                    .IsUnique();

                entity.Property(e => e.VtCode)
                    .HasColumnName("VT_CODE")
                    .HasColumnType("varchar2")
                    .HasMaxLength(20);

                entity.Property(e => e.CreatedBy)
                    .HasColumnName("CREATED_BY")
                    .HasColumnType("double");

                entity.Property(e => e.CreatedOn)
                    .HasColumnName("CREATED_ON")
                    .HasColumnType("date");

                entity.Property(e => e.ModifiedBy)
                    .HasColumnName("MODIFIED_BY")
                    .HasColumnType("double");

                entity.Property(e => e.ModifiedOn)
                    .HasColumnName("MODIFIED_ON")
                    .HasColumnType("date");

                entity.Property(e => e.UnitCode).HasColumnName("UNIT_CODE");

                entity.Property(e => e.VtevaL0001)
                    .HasColumnName("VTEVAL_0001")
                    .HasColumnType("varchar2")
                    .HasMaxLength(100);

                entity.Property(e => e.VtevaL0002)
                    .HasColumnName("VTEVAL_0002")
                    .HasColumnType("varchar2")
                    .HasMaxLength(100);

                entity.Property(e => e.VtevaL0003)
                    .HasColumnName("VTEVAL_0003")
                    .HasColumnType("varchar2")
                    .HasMaxLength(100);

                entity.Property(e => e.VtevaL0004)
                    .HasColumnName("VTEVAL_0004")
                    .HasColumnType("varchar2")
                    .HasMaxLength(100);

                entity.Property(e => e.VtevaL0005)
                    .HasColumnName("VTEVAL_0005")
                    .HasColumnType("varchar2")
                    .HasMaxLength(100);

                entity.Property(e => e.VtevaL0006)
                    .HasColumnName("VTEVAL_0006")
                    .HasColumnType("varchar2")
                    .HasMaxLength(100);

                entity.Property(e => e.VtevaL0007)
                    .HasColumnName("VTEVAL_0007")
                    .HasColumnType("varchar2")
                    .HasMaxLength(100);

                entity.Property(e => e.VtevaL0008)
                    .HasColumnName("VTEVAL_0008")
                    .HasColumnType("varchar2")
                    .HasMaxLength(100);

                entity.Property(e => e.VtevaL0009)
                    .HasColumnName("VTEVAL_0009")
                    .HasColumnType("varchar2")
                    .HasMaxLength(100);

                entity.Property(e => e.VtevaL0010)
                    .HasColumnName("VTEVAL_0010")
                    .HasColumnType("varchar2")
                    .HasMaxLength(100);

                entity.Property(e => e.VtevaL0011)
                    .HasColumnName("VTEVAL_0011")
                    .HasColumnType("varchar2")
                    .HasMaxLength(100);

                entity.Property(e => e.VtevaL0012)
                    .HasColumnName("VTEVAL_0012")
                    .HasColumnType("varchar2")
                    .HasMaxLength(100);

                entity.Property(e => e.VtevaL0013)
                    .HasColumnName("VTEVAL_0013")
                    .HasColumnType("varchar2")
                    .HasMaxLength(100);

                entity.Property(e => e.VtevaL0014)
                    .HasColumnName("VTEVAL_0014")
                    .HasColumnType("varchar2")
                    .HasMaxLength(100);

                entity.Property(e => e.VtevaL0015)
                    .HasColumnName("VTEVAL_0015")
                    .HasColumnType("varchar2")
                    .HasMaxLength(100);

                entity.Property(e => e.VtevaL0016)
                    .HasColumnName("VTEVAL_0016")
                    .HasColumnType("varchar2")
                    .HasMaxLength(100);

                entity.Property(e => e.VtevaL0017)
                    .HasColumnName("VTEVAL_0017")
                    .HasColumnType("varchar2")
                    .HasMaxLength(100);

                entity.Property(e => e.VtevaL0018)
                    .HasColumnName("VTEVAL_0018")
                    .HasColumnType("varchar2")
                    .HasMaxLength(100);

                entity.Property(e => e.VtevaL0019)
                    .HasColumnName("VTEVAL_0019")
                    .HasColumnType("varchar2")
                    .HasMaxLength(100);
            });
        }
    }
}
