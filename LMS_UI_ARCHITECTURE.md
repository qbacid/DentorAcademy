# LMS UI Architecture & Best Practices

## Overview
This document outlines the UI architecture for DentorAcademy's Learning Management System, inspired by industry-leading platforms (Moodle, Coursera, Canvas, Udemy).

## Design Principles (Based on LMS Best Practices)

### 1. **Hierarchical Navigation** (Moodle/Canvas Pattern)
- Left sidebar for primary navigation
- Breadcrumb trail for context awareness
- Tabbed interfaces for related content management

### 2. **Card-Based Layouts** (Coursera/Udemy Pattern)
- Course cards with thumbnails, progress indicators
- Grid/List view toggle for user preference
- Hover effects showing quick actions

### 3. **Progressive Disclosure** (Canvas Pattern)
- Expandable sections to reduce cognitive load
- Inline editing where possible
- Modal dialogs for focused tasks

### 4. **Drag-and-Drop** (Moodle Pattern)
- Reorder modules and content visually
- Intuitive content organization
- Visual feedback during drag operations

### 5. **Inline Preview** (Modern LMS Pattern)
- Preview content without leaving the page
- Quick view for documents/videos
- Immediate feedback on changes

## UI Component Structure

```
/Components/Pages/Admin/
├── Categories/
│   ├── CategoryList.razor          # List all categories with CRUD
│   ├── CategoryForm.razor          # Create/Edit category
│   └── CategoryTree.razor          # Hierarchical category view
│
├── Courses/
│   ├── CourseList.razor            # All courses (grid/list view)
│   ├── CourseCreate.razor          # Create new course
│   ├── CourseEdit.razor            # Edit course details
│   ├── CourseBuilder.razor         # Module & content builder
│   └── CourseSettings.razor        # Advanced settings
│
├── CourseModules/
│   ├── ModuleEditor.razor          # Inline module editing
│   ├── ModuleReorder.razor         # Drag-drop reordering
│   └── ModulePreview.razor         # Preview module
│
├── CourseContent/
│   ├── ContentUpload.razor         # Upload files to Azure Blob
│   ├── ContentEditor.razor         # Edit content details
│   ├── ContentList.razor           # List content items
│   └── QuizAssignment.razor        # Assign quiz to content
│
├── Enrollments/
│   ├── EnrollmentManager.razor     # Bulk enroll users
│   ├── EnrollmentList.razor        # View all enrollments
│   └── EnrollmentProgress.razor    # Track student progress
│
└── Instructors/
    ├── InstructorList.razor        # Manage instructors
    └── CourseAssignment.razor      # Assign courses to instructors

/Components/Pages/Instructor/
├── MyCourses.razor                 # Instructor's courses dashboard
├── CourseManagement.razor          # Manage assigned courses
└── StudentProgress.razor           # View student analytics

/Components/Pages/Student/
├── CourseCatalog.razor             # Browse available courses
├── MyCourses.razor                 # Enrolled courses
├── CoursePlayer.razor              # Watch course content
└── CourseProgress.razor            # View progress

/Components/Shared/
├── CategoryPicker.razor            # Reusable category selector
├── CourseThumbnail.razor           # Course card component
├── ModuleAccordion.razor           # Collapsible module list
├── ProgressRing.razor              # Circular progress indicator
├── FileUploader.razor              # Azure Blob file uploader
└── RichTextEditor.razor            # WYSIWYG editor
```

## Key Features by Screen

### 1. Category Management
**Pattern:** Similar to Moodle's Category Management
- **Tree View:** Hierarchical display with expand/collapse
- **Inline Actions:** Edit, Delete, Add Subcategory
- **Drag-Drop:** Reorder categories
- **Bulk Operations:** Move multiple courses between categories
- **Course Count:** Display number of courses per category

### 2. Course Builder
**Pattern:** Similar to Thinkific/Teachable Course Builder
- **Left Panel:** Module/content tree
- **Center Panel:** Content editor/preview
- **Right Panel:** Settings and properties
- **Drag-Drop:** Reorder modules and content
- **Auto-Save:** Changes saved automatically
- **Version Control:** Track changes (future)

### 3. Content Upload
**Pattern:** Similar to Canvas File Upload
- **Multi-File Upload:** Drag-drop or browse
- **Progress Indicators:** Upload progress per file
- **Type Detection:** Auto-detect content type
- **Thumbnail Generation:** Auto-generate for videos
- **Azure Blob Integration:** Direct upload to blob storage

### 4. Enrollment Management
**Pattern:** Similar to Coursera Admin Panel
- **Bulk Enrollment:** CSV import or manual selection
- **Filters:** By course, status, date range
- **Actions:** Approve, reject, extend access
- **Email Notifications:** Automated enrollment confirmations
- **Progress Tracking:** Visual progress bars

### 5. Analytics Dashboard
**Pattern:** Similar to Udemy Instructor Dashboard
- **Key Metrics:** Enrollments, completions, revenue
- **Charts:** Line, bar, pie charts for trends
- **Student Activity:** Recent activity feed
- **Course Performance:** Top performing courses
- **Export Reports:** CSV/PDF export

## Color Scheme & Branding

### Primary Colors (Dental/Medical Theme)
- **Primary:** #0066CC (Trust Blue)
- **Secondary:** #00A67E (Medical Green)
- **Accent:** #FF6B35 (Call-to-action Orange)
- **Success:** #28A745 (Completion Green)
- **Warning:** #FFC107 (Attention Yellow)
- **Danger:** #DC3545 (Error Red)

### UI States
- **Draft:** Gray (#6C757D)
- **Published:** Green (#28A745)
- **Archived:** Brown (#795548)
- **Featured:** Gold (#FFD700)

## Responsive Design

### Breakpoints
- **Mobile:** < 768px (Stack layout, bottom nav)
- **Tablet:** 768px - 1024px (Sidebar collapses)
- **Desktop:** > 1024px (Full sidebar, multi-column)

### Mobile-First Features
- Touch-friendly buttons (min 44px)
- Swipe gestures for navigation
- Bottom navigation bar
- Collapsible sections
- Simplified forms

## Accessibility (WCAG 2.1 AA)
- Keyboard navigation support
- Screen reader compatible
- High contrast mode
- Focus indicators
- ARIA labels on all interactive elements
- Alt text for images

## Performance Optimizations
- Lazy loading for course lists
- Virtual scrolling for long lists
- Image optimization (WebP format)
- CDN for static assets
- Pagination for large datasets
- Debounced search inputs

## Security Features
- Role-based access control (Admin, Instructor, Student)
- CSRF protection on all forms
- XSS prevention (sanitized inputs)
- Secure file uploads (type validation)
- Audit logging for sensitive operations

