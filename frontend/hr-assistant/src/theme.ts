export const theme = {
  colors: {
    primary: {
      main: '#7C3AED',      // Rich purple
      dark: '#5B21B6',      // Darker purple
      light: '#A78BFA',     // Light purple
      lighter: '#E9D5FF',   // Very light purple
    },
    secondary: {
      main: '#6B46C1',      // Deep purple
      light: '#8B5CF6',     // Medium purple
      lighter: '#C4B5FD',   // Soft purple
    },
    background: {
      primary: '#FFFFFF',    // Pure white
      secondary: '#FEFBFF',  // Slightly purple-tinted white
      card: '#FFFFFF',       // White cards
      accent: '#F8F4FF',     // Very light purple background
    },
    text: {
      primary: '#1F2937',    // Dark gray
      secondary: '#6B7280',  // Medium gray
      light: '#9CA3AF',      // Light gray
      accent: '#7C3AED',     // Purple text for emphasis
    },
    border: {
      light: '#E5E7EB',      // Light gray border
      medium: '#D1D5DB',     // Medium gray border
      accent: '#C4B5FD',     // Purple border
    },
    success: '#10B981',      // Green for success states
    warning: '#F59E0B',      // Amber for warnings
    error: '#EF4444',        // Red for errors
  },

  typography: {
    h1: {
      fontSize: '2.5rem',
      fontWeight: '700',
      lineHeight: '1.2',
    },
    h2: {
      fontSize: '2rem',
      fontWeight: '600',
      lineHeight: '1.3',
    },
    h3: {
      fontSize: '1.5rem',
      fontWeight: '600',
      lineHeight: '1.4',
    },
    body: {
      fontSize: '1rem',
      fontWeight: '400',
      lineHeight: '1.6',
    },
    small: {
      fontSize: '0.875rem',
      fontWeight: '400',
      lineHeight: '1.5',
    },
    caption: {
      fontSize: '0.75rem',
      fontWeight: '400',
      lineHeight: '1.4',
    },
  },

  spacing: {
    xs: '0.25rem',    // 4px
    sm: '0.5rem',     // 8px
    md: '1rem',       // 16px
    lg: '1.5rem',     // 24px
    xl: '2rem',       // 32px
    xxl: '3rem',      // 48px
  },

  borderRadius: {
    small: '0.25rem',    // 4px
    medium: '0.5rem',    // 8px
    large: '1rem',       // 16px
    round: '50%',
  },

  shadows: {
    small: '0 1px 3px 0 rgba(124, 58, 237, 0.1), 0 1px 2px 0 rgba(124, 58, 237, 0.06)',
    medium: '0 4px 6px -1px rgba(124, 58, 237, 0.1), 0 2px 4px -1px rgba(124, 58, 237, 0.06)',
    large: '0 10px 15px -3px rgba(124, 58, 237, 0.1), 0 4px 6px -2px rgba(124, 58, 237, 0.05)',
  },

  transitions: {
    fast: '0.15s ease',
    normal: '0.2s ease',
    slow: '0.3s ease',
  },
};
