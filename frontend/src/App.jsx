import React, { useState, useEffect } from 'react';
import { 
  GraduationCap, 
  Users, 
  BookOpen, 
  ShieldCheck, 
  Plus, 
  Search, 
  Pencil, 
  Trash2, 
  Lock, 
  RefreshCw, 
  X, 
  CheckCircle2, 
  AlertCircle 
} from 'lucide-react';

const INITIAL_MOCK_STUDENTS = [
  { id: 1, name: 'Rahul Sharma', email: 'rahul.sharma@example.com', age: 22, course: 'Computer Science', createdDate: '2026-07-11T10:30:00Z' },
  { id: 2, name: 'Priya Patel', email: 'priya.patel@example.com', age: 21, course: 'Information Technology', createdDate: '2026-07-13T14:15:00Z' },
  { id: 3, name: 'Aman Verma', email: 'aman.verma@example.com', age: 23, course: 'Software Engineering', createdDate: '2026-07-16T09:00:00Z' },
  { id: 4, name: 'Sneha Gupta', email: 'sneha.gupta@example.com', age: 20, course: 'Data Science', createdDate: '2026-07-19T11:45:00Z' }
];

export default function App() {
  const [students, setStudents] = useState(INITIAL_MOCK_STUDENTS);
  const [searchQuery, setSearchQuery] = useState('');
  const [jwtToken, setJwtToken] = useState('eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.e30.dummy_jwt_token_zest_india');
  const [isLoggedIn, setIsLoggedIn] = useState(true);
  const [useLiveApi, setUseLiveApi] = useState(false);
  const [apiBaseUrl, setApiBaseUrl] = useState('http://localhost:5000/api');

  // Modal States
  const [isStudentModalOpen, setIsStudentModalOpen] = useState(false);
  const [isAuthModalOpen, setIsAuthModalOpen] = useState(false);
  const [editingStudent, setEditingStudent] = useState(null);

  // Form States
  const [formData, setFormData] = useState({ name: '', email: '', age: 20, course: '' });
  const [authData, setAuthData] = useState({ username: 'admin', password: 'Admin@123' });
  const [alert, setAlert] = useState(null);

  const showAlert = (message, type = 'success') => {
    setAlert({ message, type });
    setTimeout(() => setAlert(null), 4000);
  };

  // Fetch Students from API or Local
  const fetchStudents = async () => {
    if (useLiveApi) {
      try {
        const res = await fetch(`${apiBaseUrl}/students`, {
          headers: { 'Authorization': `Bearer ${jwtToken}` }
        });
        const data = await res.json();
        if (data.success) {
          setStudents(data.data);
          showAlert('Students fetched from Web API!');
        } else {
          showAlert(data.message || 'Failed to fetch', 'error');
        }
      } catch (err) {
        showAlert('API connection failed. Switching to Local Demo mode.', 'error');
        setUseLiveApi(false);
      }
    }
  };

  useEffect(() => {
    if (useLiveApi) {
      fetchStudents();
    }
  }, [useLiveApi]);

  // Handle Form Submission (Add or Update)
  const handleSubmitStudent = async (e) => {
    e.preventDefault();
    if (!formData.name || !formData.email || !formData.course) {
      showAlert('Please fill all required fields.', 'error');
      return;
    }

    if (useLiveApi) {
      try {
        const url = editingStudent 
          ? `${apiBaseUrl}/students/${editingStudent.id}` 
          : `${apiBaseUrl}/students`;
        const method = editingStudent ? 'PUT' : 'POST';

        const res = await fetch(url, {
          method,
          headers: {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${jwtToken}`
          },
          body: JSON.stringify(formData)
        });
        const data = await res.json();
        if (data.success) {
          showAlert(editingStudent ? 'Student updated!' : 'Student added successfully!');
          fetchStudents();
          closeModal();
        } else {
          showAlert(data.message || 'Operation failed', 'error');
        }
      } catch (err) {
        showAlert('API Error: ' + err.message, 'error');
      }
    } else {
      // Local State Update
      if (editingStudent) {
        setStudents(students.map(s => s.id === editingStudent.id ? { ...s, ...formData } : s));
        showAlert('Student updated successfully!');
      } else {
        const maxId = students.reduce((max, s) => (typeof s.id === 'number' && s.id < 1000000 ? Math.max(max, s.id) : max), 0);
        const newStudent = {
          id: maxId + 1,
          ...formData,
          createdDate: new Date().toISOString()
        };
        setStudents([newStudent, ...students]);
        showAlert('Student added successfully!');
      }
      closeModal();
    }
  };

  // Delete Student
  const handleDeleteStudent = async (id) => {
    if (!confirm('Are you sure you want to delete this student?')) return;

    if (useLiveApi) {
      try {
        const res = await fetch(`${apiBaseUrl}/students/${id}`, {
          method: 'DELETE',
          headers: { 'Authorization': `Bearer ${jwtToken}` }
        });
        const data = await res.json();
        if (data.success) {
          showAlert('Student deleted successfully!');
          fetchStudents();
        }
      } catch (err) {
        showAlert('Failed to delete student', 'error');
      }
    } else {
      setStudents(students.filter(s => s.id !== id));
      showAlert('Student deleted successfully!');
    }
  };

  // Open Edit Modal
  const openEditModal = (student) => {
    setEditingStudent(student);
    setFormData({
      name: student.name,
      email: student.email,
      age: student.age,
      course: student.course
    });
    setIsStudentModalOpen(true);
  };

  // Open Create Modal
  const openCreateModal = () => {
    setEditingStudent(null);
    setFormData({ name: '', email: '', age: 20, course: 'Computer Science' });
    setIsStudentModalOpen(true);
  };

  const closeModal = () => {
    setIsStudentModalOpen(false);
    setEditingStudent(null);
  };

  // Auth Login Handler
  const handleAuthLogin = async (e) => {
    e.preventDefault();
    if (useLiveApi) {
      try {
        const res = await fetch(`${apiBaseUrl}/auth/login`, {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({ usernameOrEmail: authData.username, password: authData.password })
        });
        const data = await res.json();
        if (data.success) {
          setJwtToken(data.data.token);
          setIsLoggedIn(true);
          setIsAuthModalOpen(false);
          showAlert('JWT Authenticated Successfully!');
        } else {
          showAlert(data.message, 'error');
        }
      } catch (err) {
        showAlert('Login API failed.', 'error');
      }
    } else {
      setIsLoggedIn(true);
      setIsAuthModalOpen(false);
      showAlert('Logged in (Demo Mode)');
    }
  };

  // Filtered Students
  const filteredStudents = students.filter(s => 
    s.name.toLowerCase().includes(searchQuery.toLowerCase()) ||
    s.email.toLowerCase().includes(searchQuery.toLowerCase()) ||
    s.course.toLowerCase().includes(searchQuery.toLowerCase())
  );

  const uniqueCourses = new Set(students.map(s => s.course)).size;

  return (
    <div className="app-container">
      {/* Toast Alert */}
      {alert && (
        <div style={{
          position: 'fixed', top: '20px', right: '20px', zIndex: 2000,
          background: alert.type === 'error' ? 'rgba(239,68,68,0.9)' : 'rgba(16,185,129,0.9)',
          color: '#fff', padding: '12px 20px', borderRadius: '12px', backdropFilter: 'blur(10px)',
          boxShadow: '0 8px 30px rgba(0,0,0,0.4)', display: 'flex', alignItems: 'center', gap: '10px'
        }}>
          {alert.type === 'error' ? <AlertCircle size={20} /> : <CheckCircle2 size={20} />}
          <span style={{ fontWeight: 600 }}>{alert.message}</span>
        </div>
      )}

      {/* Top Navigation */}
      <header className="navbar">
        <div className="brand">
          <div className="brand-icon">
            <GraduationCap size={28} />
          </div>
          <div>
            <h1 className="brand-title">Zest India IT System</h1>
            <p className="brand-sub">ASP.NET Core Web API Management Dashboard</p>
          </div>
        </div>

        <div className="nav-actions">
          <div className={`jwt-badge ${isLoggedIn ? '' : 'disconnected'}`}>
            <ShieldCheck size={16} />
            <span>{isLoggedIn ? 'JWT Secured' : 'Unauthenticated'}</span>
          </div>

          <button 
            className="btn btn-secondary"
            onClick={() => setUseLiveApi(!useLiveApi)}
            title="Toggle Live Web API / Demo Mode"
          >
            <RefreshCw size={16} />
            {useLiveApi ? 'Live API Mode' : 'Local Demo Mode'}
          </button>

          <button className="btn btn-primary" onClick={() => setIsAuthModalOpen(true)}>
            <Lock size={16} />
            JWT Login
          </button>
        </div>
      </header>

      {/* Statistics Cards */}
      <div className="stats-grid">
        <div className="stat-card">
          <div className="stat-icon purple"><Users /></div>
          <div className="stat-info">
            <h4>Total Students</h4>
            <div className="stat-value">{students.length}</div>
          </div>
        </div>

        <div className="stat-card">
          <div className="stat-icon green"><BookOpen /></div>
          <div className="stat-info">
            <h4>Active Courses</h4>
            <div className="stat-value">{uniqueCourses}</div>
          </div>
        </div>

        <div className="stat-card">
          <div className="stat-icon blue"><ShieldCheck /></div>
          <div className="stat-info">
            <h4>API Status</h4>
            <div className="stat-value" style={{ fontSize: '1.2rem' }}>
              {useLiveApi ? 'Connected (5000)' : 'Active (Demo)'}
            </div>
          </div>
        </div>
      </div>

      {/* Main Content Area */}
      <main className="main-card">
        <div className="toolbar">
          <div className="search-box">
            <Search size={18} color="#94a3b8" />
            <input 
              type="text" 
              placeholder="Search by student name, email, or course..."
              value={searchQuery}
              onChange={(e) => setSearchQuery(e.target.value)}
            />
          </div>

          <div className="actions-right">
            <button className="btn btn-primary" onClick={openCreateModal}>
              <Plus size={18} />
              Add New Student
            </button>
          </div>
        </div>

        {/* Student Table */}
        <div className="table-container">
          <table>
            <thead>
              <tr>
                <th>ID</th>
                <th>Student Name</th>
                <th>Email Address</th>
                <th>Age</th>
                <th>Course</th>
                <th>Registration Date</th>
                <th style={{ textAlign: 'right' }}>Actions</th>
              </tr>
            </thead>
            <tbody>
              {filteredStudents.length > 0 ? (
                filteredStudents.map((s) => (
                  <tr key={s.id}>
                    <td>#{s.id}</td>
                    <td className="student-name">{s.name}</td>
                    <td>{s.email}</td>
                    <td>{s.age} yrs</td>
                    <td><span className="course-badge">{s.course}</span></td>
                    <td>{new Date(s.createdDate).toLocaleDateString()}</td>
                    <td style={{ textAlign: 'right' }}>
                      <div className="action-btns" style={{ justifyContent: 'flex-end' }}>
                        <button className="btn-icon" onClick={() => openEditModal(s)} title="Edit Student">
                          <Pencil size={16} />
                        </button>
                        <button className="btn-icon delete" onClick={() => handleDeleteStudent(s.id)} title="Delete Student">
                          <Trash2 size={16} />
                        </button>
                      </div>
                    </td>
                  </tr>
                ))
              ) : (
                <tr>
                  <td colSpan={7} style={{ textAlign: 'center', padding: '3rem', color: '#94a3b8' }}>
                    No student records found matching your query.
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </main>

      {/* Student Add/Edit Modal */}
      {isStudentModalOpen && (
        <div className="modal-overlay">
          <div className="modal-content">
            <div className="modal-header">
              <h3 className="modal-title">{editingStudent ? 'Edit Student Details' : 'Add New Student'}</h3>
              <button className="btn-icon" onClick={closeModal}><X size={18} /></button>
            </div>
            <form onSubmit={handleSubmitStudent}>
              <div className="form-group">
                <label>Full Name *</label>
                <input 
                  type="text" 
                  className="form-control" 
                  required
                  placeholder="e.g. Rahul Sharma"
                  value={formData.name}
                  onChange={(e) => setFormData({ ...formData, name: e.target.value })}
                />
              </div>

              <div className="form-group">
                <label>Email Address *</label>
                <input 
                  type="email" 
                  className="form-control" 
                  required
                  placeholder="e.g. rahul@example.com"
                  value={formData.email}
                  onChange={(e) => setFormData({ ...formData, email: e.target.value })}
                />
              </div>

              <div style={{ display: 'grid', gridTemplateColumns: '1fr 2fr', gap: '1rem' }}>
                <div className="form-group">
                  <label>Age *</label>
                  <input 
                    type="number" 
                    className="form-control" 
                    required
                    min={1} max={120}
                    value={formData.age}
                    onChange={(e) => setFormData({ ...formData, age: parseInt(e.target.value) || 20 })}
                  />
                </div>

                <div className="form-group">
                  <label>Course *</label>
                  <input 
                    type="text" 
                    className="form-control" 
                    required
                    placeholder="e.g. Computer Science"
                    value={formData.course}
                    onChange={(e) => setFormData({ ...formData, course: e.target.value })}
                  />
                </div>
              </div>

              <div className="modal-footer">
                <button type="button" className="btn btn-secondary" onClick={closeModal}>Cancel</button>
                <button type="submit" className="btn btn-primary">
                  {editingStudent ? 'Save Changes' : 'Create Student'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* JWT Login Modal */}
      {isAuthModalOpen && (
        <div className="modal-overlay">
          <div className="modal-content">
            <div className="modal-header">
              <h3 className="modal-title">JWT Authentication Login</h3>
              <button className="btn-icon" onClick={() => setIsAuthModalOpen(false)}><X size={18} /></button>
            </div>
            <form onSubmit={handleAuthLogin}>
              <div className="form-group">
                <label>Username / Email</label>
                <input 
                  type="text" 
                  className="form-control"
                  value={authData.username}
                  onChange={(e) => setAuthData({ ...authData, username: e.target.value })}
                />
              </div>
              <div className="form-group">
                <label>Password</label>
                <input 
                  type="password" 
                  className="form-control"
                  value={authData.password}
                  onChange={(e) => setAuthData({ ...authData, password: e.target.value })}
                />
              </div>

              <div className="modal-footer">
                <button type="button" className="btn btn-secondary" onClick={() => setIsAuthModalOpen(false)}>Close</button>
                <button type="submit" className="btn btn-primary">Authenticate & Get Token</button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}
