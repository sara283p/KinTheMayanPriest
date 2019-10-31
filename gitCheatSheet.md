# Git Cheatsheet

```bash
git pull
```

Retrieves up-to-date files from the **remote** repository. Should be the first command launched every time you start working on anything.



```bash
git add <file1> [<file2> ...]
```

Adds the specified files to the **local** repository.

***Note*: it is possible to specify also folders, not necessarily single files**



```bash
git rm <file1> [<file2> ...]
```

Removes the specified files from the **local** repository.

***Note*: as for the *add* command, folders may be specified instead of files**



```bash
git commit -m "Message for the commit"
```

Commits all the modified files to the **local** repository.



```bash
git push <remote_name> <branch_name>
```

Pushes all the pending commits from the **local** repository to the **remote** repository.

Usually, <remote_name> is *origin* and <branch_name> is *master*.



```bash
git status
```

Prints the status of the **local** repository, telling which files are modified, added or removed. Also tells which of these modifications are already committed.



```bash
git branch
```

Prints the list of all the existing branches in the **local** repository.



```bash
git branch <branch_name>
```

Creates a new branch with the specified name in the **local** repository.



```bash
git branch -d <branch_name>
```

Deletes the specified branch from the **local** repository.



```bash
git checkout <branch_name>
```

Switches the context into the specified branch in the **local** repository.



```bash
git merge <branch_name>
```

Merges the modifications from the specified branch into the current branch.



```bash
git remote add <remote_name> <remote_repository_url>
```

Links the **local** repository with the **remote** repository located at the specified url.



```bash
git stash save
```

Saves the modified files locally, and cleans the **local** repository to have no pending modifications.



```bash
git stash pop
```

Retrieves modifications from the last stash saved.



```bash
git stash list
```

Prints the list of all the existing stashes saved.



```bash
git stash drop <stash_ID>
```

Deletes the specified stash.



```bash
git revert <commit>
```

Reverts the **local** repository to the specified commit. To revert only the last commit, use *HEAD* as < commit>. In order to revert the **remote** repository, simply push the modifications.

***Note*: this command deletes the modifications of all the files**



```bash
git reset <commit>
```

Similar to *revert* command, but it takes the modifications to the files.



```bash
git diff
```

Shows the differences between unstaged files and the last version available.



```bash
git diff -staged
```

Shows the differences between staged files and the last version available.



```bash
git diff <branch1> <branch2>
```

Shows differences between files belonging to the specified branches.



## Git configuration commands

```bash
git config --global user.email <e-mail_address>
```

Saves the e-mail address used for commits.



```bash
git config --global user.name <username>
```

Saves the username used for commits.



```bash
git config --global credential.helper store
```

After the first operation on the **remote** repository, credentials are cached and never asked again.