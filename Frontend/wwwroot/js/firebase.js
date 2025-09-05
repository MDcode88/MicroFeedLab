// Questo file gestisce l'interazione diretta con Firebase Auth SDK

// Inizializza Firebase con la configurazione da index.html
function initializeFirebase() {
    if (typeof firebase !== 'undefined' && firebaseConfig) {
        firebase.initializeApp(firebaseConfig);
        console.log("Firebase Initialized");
        return true;
    }
    return false;
}

// Funzione per registrare un nuovo utente
async function signUp(email, password) {
    try {
        const userCredential = await firebase.auth().createUserWithEmailAndPassword(email, password);
        return { success: true, userId: userCredential.user.uid };
    } catch (error) {
        return { success: false, message: error.message };
    }
}

// Funzione per effettuare il login
async function signIn(email, password) {
    try {
        const userCredential = await firebase.auth().signInWithEmailAndPassword(email, password);
        return { success: true, userId: userCredential.user.uid };
    } catch (error) {
        return { success: false, message: error.message };
    }
}

// Funzione per il logout
async function signOut() {
    try {
        await firebase.auth().signOut();
        return { success: true };
    } catch (error) {
        return { success: false, message: error.message };
    }
}

// Funzione per ottenere il token JWT dell'utente corrente
async function getJwtToken() {
    const user = firebase.auth().currentUser;
    if (user) {
        return await user.getIdToken(true); // true forza il refresh se scaduto
    }
    return null;
}
