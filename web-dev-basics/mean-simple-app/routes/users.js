const express = require('express');
const router = express.Router();

// Register
router.get('/register', (req, res, next) => {
    console.log('6');
    res.send('REGISTER');
});

// Authenticate
router.get('/authenticate', (req, res, next) => {
    res.send('AUTH');
});


// Profile
router.get('/profile', (req, res, next) => {
    res.send('PROFILE');
});

// Validate
router.get('/validate', (req, res, next) => {
    res.send('VALID');
});

module.exports = router;
